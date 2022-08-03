using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace Trader
{
    internal class DataBaseManagment
    {
        // Closed position data
        static readonly string connectionStringStreaming = "Data Source=DESKTOP-LSTSCHE\\SQLEXPRESS;" +
                    "Initial Catalog=TraderDB; Integrated Security=True";
        public static void CreateTable(string tableName)
        {
            try
            {                
                // Utworzenie głównej tabeli
                string sqlQuery = "CREATE TABLE [" + tableName +"](" +
                    "rId int NOT NULL IDENTITY(1, 1)," +
                    "rOpen  decimal(7, 5) NOT NULL, " +
                    "rHigh  decimal(7, 5) NOT NULL, " +
                    "rLow   decimal(7, 5) NOT NULL," +
                    "rClose decimal(7, 5) NOT NULL, " +
                    "rSMAs  decimal(7, 5) NOT NULL, " +
                    "rSMAb  decimal(7, 5) NOT NULL, " +
                    "rMACD  decimal(7, 5) NOT NULL, " +
                    "rSignalLine  decimal(7, 5) NOT NULL, " +
                    "rOpenPrice   decimal(7, 5) NOT NULL, " +
                    "rClosePrice  decimal(7, 5) NOT NULL); " +
                    "" +
                    "CREATE TABLE [" + tableName + "Name](" +
                    "rId int NOT NULL IDENTITY(1, 1)," +
                    "rSymbol  decimal(7, 5) NOT NULL, " +
                    "rBuySell  decimal(7, 5) NOT NULL, " +
                    "rOpenPrice decimal(7, 5) NOT NULL, " +
                    "rClosePrice decimal(7, 5) NOT NULL, " +
                    "rProfit  decimal(7, 2) NOT NULL);";

                SqlConnection con = new SqlConnection(connectionStringStreaming);

                con.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch
            {
                MessageBox.Show("Error while creating table");
            }

        }
        public static void InsertIntoTable(string tableName)
        {            
            try
            {
                string sqlQuery;

                DataTable dt = new DataTable();

                decimal rOpen;
                decimal rHigh;
                decimal rLow;
                decimal rClose;
                decimal rSMAs;
                decimal rSMAb;
                decimal rMACD;
                decimal rSignalLine;
                decimal rOpenPrice;
                decimal rClosePrice;

                // Add columns
                dt.Columns.Add(new DataColumn("rOpen", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rHigh", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rLow", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rClose", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rSMAs", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rSMAb", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rMACD", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rSignalLine", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rOpenPrice", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rClosePrice", typeof(decimal)));

                //Add rows
                for (int i = 0; i < Program.candleColumns; i++)
                {
                    rOpen = (decimal)Program.closedPositionMatrix[0, i];
                    rHigh = (decimal)Program.closedPositionMatrix[1, i];
                    rLow = (decimal)Program.closedPositionMatrix[2, i];
                    rClose = (decimal)Program.closedPositionMatrix[3, i];
                    rSMAs = (decimal)Program.closedPositionMatrix[4, i];
                    rSMAb = (decimal)Program.closedPositionMatrix[5, i];
                    rMACD = (decimal)Program.closedPositionMatrix[6, i];
                    rSignalLine = (decimal)Program.closedPositionMatrix[7, i];
                    rOpenPrice = (decimal)Program.closedPositionMatrix[8, i];
                    rClosePrice = (decimal)Program.closedPositionMatrix[9, i];

                    dt.Rows.Add(rOpen, rHigh, rLow, rClose, rSMAs, rSMAb, rMACD, rSignalLine, rOpenPrice, rClosePrice);

                    // Jeżeli następny wiersz macierzy jest pusty przerwij uzupełnianie tabeli
                    if (Program.closedPositionMatrix[0, i + 1] == 0)
                        break;
                }

                SqlConnection con = new SqlConnection(connectionStringStreaming);

                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy objbulk = new SqlBulkCopy(con);

                //assign Destination table name  
                objbulk.DestinationTableName = "[" + tableName + "]";

                // MAPPING the columns of Datatable to Database Table
                objbulk.ColumnMappings.Add("rOpen", "rOpen");
                objbulk.ColumnMappings.Add("rHigh", "rHigh");
                objbulk.ColumnMappings.Add("rLow", "rLow");
                objbulk.ColumnMappings.Add("rClose", "rClose");
                objbulk.ColumnMappings.Add("rSMAs", "rSMAs");
                objbulk.ColumnMappings.Add("rSMAb", "rSMAb");
                objbulk.ColumnMappings.Add("rMACD", "rMACD");
                objbulk.ColumnMappings.Add("rSignalLine", "rSignalLine");
                objbulk.ColumnMappings.Add("rOpenPrice", "rOpenPrice");
                objbulk.ColumnMappings.Add("rClosePrice", "rClosePrice");

                con.Open();

                //insert bulk Records into DataBase/ Save data from matrix
                objbulk.WriteToServer(dt);



                // Save data from vector
                sqlQuery = "INSERT INTO [" + tableName + "Name](" +
                        "rSymbol, rBuySell, rOpenPrice, rClosePrice, rProfit) " +
                        "VALUES(" +
                        "@rpSymbol, @rpBuySell, @rpOpenPrice, @rpClosePrice, @rpProfit);";


                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                var rParameter = new SqlParameter("rpSymbol", System.Data.SqlDbType.Decimal);
                rParameter.Value = Program.closedPositionVector[0];
                cmd.Parameters.Add(rParameter);
                rParameter = new SqlParameter("rpBuySell", System.Data.SqlDbType.Decimal);
                rParameter.Value = Program.closedPositionVector[1];
                cmd.Parameters.Add(rParameter);
                rParameter = new SqlParameter("rpOpenPrice", System.Data.SqlDbType.Decimal);
                rParameter.Value = Program.closedPositionVector[2];
                cmd.Parameters.Add(rParameter);
                rParameter = new SqlParameter("rpClosePrice", System.Data.SqlDbType.Decimal);
                rParameter.Value = Program.closedPositionVector[3];
                cmd.Parameters.Add(rParameter);
                rParameter = new SqlParameter("rpProfit", System.Data.SqlDbType.Decimal);
                rParameter.Value = Program.closedPositionVector[4];
                cmd.Parameters.Add(rParameter);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while inserting into table.\n" + ex);
            }
        }
        public static void RetrieveDataFromTable(string tableName)
        {
            string sqlQuery;
            SqlCommand cmd;
            SqlDataReader reader;

            // Clear matrix
            for (int i = 0; i < 8; i++) // save SMA small and big
            {
                for (int j = 0; j < Program.candleColumns; j++)
                {
                    Program.closedPositionMatrix[i, j] = 0;
                }
            }

            // Retrieve table and save data for chart in matrix            
            SqlConnection con = new SqlConnection(connectionStringStreaming);
            con.Open();

            sqlQuery = "SELECT rOpen, rHigh, rLow, rClose, rSMAs, rSMAb, rMACD, rSignalLine, rOpenPrice, rClosePrice " +
                "FROM [" + tableName + "];";
            cmd = new SqlCommand(sqlQuery, con);
            reader = cmd.ExecuteReader();

            for (int i = 0; i < Program.candleColumns; i++)
            {
                if (reader.Read())
                {
                    Program.closedPositionMatrix[0, i] = (double)reader.GetDecimal(0);
                    Program.closedPositionMatrix[1, i] = (double)reader.GetDecimal(1);
                    Program.closedPositionMatrix[2, i] = (double)reader.GetDecimal(2);
                    Program.closedPositionMatrix[3, i] = (double)reader.GetDecimal(3);
                    Program.closedPositionMatrix[4, i] = (double)reader.GetDecimal(4);
                    Program.closedPositionMatrix[5, i] = (double)reader.GetDecimal(5);
                    Program.closedPositionMatrix[6, i] = (double)reader.GetDecimal(6);
                    Program.closedPositionMatrix[7, i] = (double)reader.GetDecimal(7);
                    Program.closedPositionMatrix[8, i] = (double)reader.GetDecimal(8);
                    Program.closedPositionMatrix[9, i] = (double)reader.GetDecimal(9);
                }
                else
                {
                    reader.Close();
                    break;
                }
            }

            // Retrieve table and save data for chart title in vector
            sqlQuery = "SELECT rSymbol, rBuySell, rOpenPrice, rClosePrice, rProfit " +
                    "FROM [" + tableName + "Name];";

            cmd = new SqlCommand(sqlQuery, con);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Program.closedPositionVector[0] = (double)reader.GetDecimal(0);
                Program.closedPositionVector[1] = (double)reader.GetDecimal(1);
                Program.closedPositionVector[2] = (double)reader.GetDecimal(2);
                Program.closedPositionVector[3] = (double)reader.GetDecimal(3);
                Program.closedPositionVector[4] = (double)reader.GetDecimal(4);
            }
            reader.Close();

        }
        public static void DeleteTable(string tableName)
        {
            try
            {                               
                string sqlQuery = "DROP TABLE [" + tableName + "];" +
                    "" +
                    "DROP TABLE [" + tableName + "Name];";

                SqlConnection con = new SqlConnection(connectionStringStreaming);

                con.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting table! \n" + ex);
            }

        }
        public static void GetAllTablesNameInDB()
        {
            try
            {
                string tableName;
                SqlCommand cmd;
                SqlDataReader reader;
                string sqlQuery = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;";

                SqlConnection con = new SqlConnection(connectionStringStreaming);
                con.Open();

                cmd = new SqlCommand(sqlQuery, con);
                reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    tableName = reader.GetString(0);

                    Form1.instance.addToSavedClosedPositionList(tableName);
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting all tables name from DB!\n" + ex);
            }
        }


        // Historical candles data
        static readonly string connectionStringHistory = "Data Source=DESKTOP-LSTSCHE\\SQLEXPRESS;" +
                    "Initial Catalog=HistoricalCandlesDB; Integrated Security=True";
        public static void CreateHistoricalCandlesTable()
        {
            try
            { 
                DateTime date = DateTime.Now;
                string month;
                string day;
                string tableName;

                if (date.Month < 10)
                    month = "0" + date.Month;
                else
                    month = date.Month.ToString();

                if (date.Day < 10)
                    day = "0" + date.Day;
                else
                    day = date.Day.ToString();

                SqlConnection con = new SqlConnection(connectionStringHistory);
                con.Open();

                SqlCommand cmd;
                for (int n = 0; n < Program.symbol.Length; n++)
                {
                    
                    //tableName = Program.symbol[0] + "_" + date.Year + "_" + month + "_" + day;
                    tableName = Program.symbol[n] + "_" + date.Year + "_" + month + "_" + day;

                    // Utworzenie głównej tabeli
                    string sqlQuery = "CREATE TABLE [" + tableName + "](" +
                        "rId int NOT NULL IDENTITY(1, 1)," +
                        "rOpen  decimal(7, 5) NOT NULL, " +
                        "rHigh  decimal(7, 5) NOT NULL, " +
                        "rLow   decimal(7, 5) NOT NULL," +
                        "rClose decimal(7, 5) NOT NULL, " +
                        "rSMAs  decimal(7, 5) NOT NULL, " +
                        "rSMAb  decimal(7, 5) NOT NULL, " +
                        "rMACD  decimal(7, 5) NOT NULL, " +
                        "rSignalLine  decimal(7, 5) NOT NULL);";
                
                    cmd = new SqlCommand(sqlQuery, con);
                    cmd.ExecuteNonQuery();

                    // Wprowadzenie danych do głównej tabeli 
                    InsertHistoricalCandlesDataIntoTables(tableName, n);

                // Wprowadzenie danych o tabeli do tabeli HistoricalCandlesData
                InsertRawIntoHistoricalCandlesDataTable(tableName, Program.symbol[n]);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while creating historical candles table!\n" + ex);
            }
}
        public static void InsertRawIntoHistoricalCandlesDataTable(string tableName, string symbol)
        {
            try 
            { 
                string sqlQuery;

                SqlConnection con = new SqlConnection(connectionStringHistory);
                con.Open();

                SqlCommand cmd;

                sqlQuery = "INSERT INTO [aHistoricalCandles](" +
                            "rSymbol, rTableName) " +
                            "VALUES(" +
                            "@rpSymbol, @rpTableName);";

                cmd = new SqlCommand(sqlQuery, con);

                var rParameterSymbol = new SqlParameter("rpSymbol", System.Data.SqlDbType.NVarChar);
                rParameterSymbol.Value = symbol;
                cmd.Parameters.Add(rParameterSymbol);

                var rParameterTableName = new SqlParameter("rpTableName", System.Data.SqlDbType.NVarChar);
                rParameterTableName.Value = tableName;
                cmd.Parameters.Add(rParameterTableName);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch
            {
                MessageBox.Show("Error while inserting raw into historical candles data table!");
            }
}
        public static void InsertHistoricalCandlesDataIntoTables( string tableName, int symbol)
        {
            try
            {
                DataTable dt = new DataTable();

                decimal rOpen;
                decimal rHigh;
                decimal rLow;
                decimal rClose;
                decimal rSMAs;
                decimal rSMAb;
                decimal rMACD;
                decimal rSignalLine;

                // Add columns
                dt.Columns.Add(new DataColumn("rOpen",  typeof(decimal)));
                dt.Columns.Add(new DataColumn("rHigh",  typeof(decimal)));
                dt.Columns.Add(new DataColumn("rLow",   typeof(decimal)));
                dt.Columns.Add(new DataColumn("rClose", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rSMAs", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rSMAb", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rMACD", typeof(decimal)));
                dt.Columns.Add(new DataColumn("rSignalLine", typeof(decimal)));

                //Add rows
                for (int i = 0; i < Program.candleColumns; i++)
                {
                    rOpen = (decimal)Program.candlesMatrix[symbol, 0, i];
                    rHigh = (decimal)Program.candlesMatrix[symbol, 1, i];
                    rLow = (decimal)Program.candlesMatrix[symbol, 2, i];
                    rClose = (decimal)Program.candlesMatrix[symbol, 3, i];
                    rSMAs = (decimal)Program.candlesMatrix[symbol, 9, i];
                    rSMAb = (decimal)Program.candlesMatrix[symbol, 10, i];
                    rMACD = (decimal)Program.candlesMatrix[symbol, 11, i];
                    rSignalLine = (decimal)Program.candlesMatrix[symbol, 12, i];

                    dt.Rows.Add(rOpen, rHigh, rLow, rClose, rSMAs, rSMAb, rMACD, rSignalLine);

                    // Jeżeli następny wiersz macierzy jest pusty przerwij uzupełnianie tabeli
                    if (Program.candlesMatrix[symbol, 0, i + 1] == 0)
                        break;
                }

                SqlConnection con = new SqlConnection(connectionStringHistory);

                //create object of SqlBulkCopy which help to insert  
                SqlBulkCopy objbulk = new SqlBulkCopy(con);

                //assign Destination table name  
                objbulk.DestinationTableName = tableName;

                // MAPPING the columns of Datatable to Database Table
                objbulk.ColumnMappings.Add("rOpen", "rOpen");
                objbulk.ColumnMappings.Add("rHigh", "rHigh");
                objbulk.ColumnMappings.Add("rLow", "rLow");
                objbulk.ColumnMappings.Add("rClose", "rClose");
                objbulk.ColumnMappings.Add("rSMAs", "rSMAs");
                objbulk.ColumnMappings.Add("rSMAb", "rSMAb");
                objbulk.ColumnMappings.Add("rMACD", "rMACD");
                objbulk.ColumnMappings.Add("rSignalLine", "rSignalLine");

                con.Open();
                //insert bulk Records into DataBase.  
                objbulk.WriteToServer(dt);
                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while inserting historical candles data into tables!\n" + ex);
            }
        }
        public static void RetrieveDataFromHistoricalCandlesTable(string tableName, int symbol)
        {
            string sqlQuery;
            SqlCommand cmd;
            SqlDataReader reader;

            // Clear matrix
            for (int i = 0; i < Program.symbol.Length; i++) // save SMA small and big
            {
                for (int j = 0; j < Program.candleColumns; j++)
                {
                    Program.candlesMatrix[symbol, i, j] = 0;
                }
            }

            // Retrieve table and save data for chart in matrix            
            SqlConnection con = new SqlConnection(connectionStringHistory);
            con.Open();
            
            sqlQuery = "SELECT rOpen, rHigh, rLow, rClose, rSMAs, rSMAb, rMACD, rSignalLine " +
                "FROM [" + tableName + "];";
            cmd = new SqlCommand(sqlQuery, con);
            reader = cmd.ExecuteReader();

            for (int i = 0; i < Program.candleColumns; i++)
            {
                if (reader.Read())
                {
                    Program.candlesMatrix[symbol, 0, i] = (double)reader.GetDecimal(0);
                    Program.candlesMatrix[symbol, 1, i] = (double)reader.GetDecimal(1);
                    Program.candlesMatrix[symbol, 2, i] = (double)reader.GetDecimal(2);
                    Program.candlesMatrix[symbol, 3, i] = (double)reader.GetDecimal(3);
                    Program.candlesMatrix[symbol, 9, i] = (double)reader.GetDecimal(4);
                    Program.candlesMatrix[symbol, 10, i] = (double)reader.GetDecimal(5);
                    Program.candlesMatrix[symbol, 11, i] = (double)reader.GetDecimal(6);
                    Program.candlesMatrix[symbol, 12, i] = (double)reader.GetDecimal(7);
                }
                else
                {
                    reader.Close();
                    break;
                }
            }
            con.Close();
        }
        public static void DeleteHistoricalCandlesTable(string tableName)
        {
            try
            {
                // Delete table with data
                string sqlQuery = "DROP TABLE [" + tableName + "];";

                SqlConnection con = new SqlConnection(connectionStringHistory);

                con.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                cmd.ExecuteNonQuery();

                // Delete row represented deleted table with data
                sqlQuery = "DELETE FROM [aHistoricalCandles] WHERE rTableName = '" + tableName + "';";

                cmd = new SqlCommand(sqlQuery, con);

                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting table\n" + ex);
            }

        }
        public static void GetAllHistoricalTablesNameInDB()
        {
            try
            {
                string tableName;
                SqlCommand cmd;
                SqlDataReader reader;
                string sqlQuery = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;";

                SqlConnection con = new SqlConnection(connectionStringHistory);
                con.Open();

                cmd = new SqlCommand(sqlQuery, con);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tableName = reader.GetString(0);

                    Form1.instance.addToHistoricalDataList(tableName);
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting all tables name from DB!\n" + ex);
            }
        }
    }
}
