using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trader;

using xAPI.Codes;
using xAPI.Commands;
using xAPI.Errors;
using xAPI.Records;
using xAPI.Responses;
using xAPI.Sync;
using xAPI.Streaming;

namespace Trader
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        // Properties
        SyncAPIConnector connector;
        LoginResponse loginResponse;
        int selectedSymbol = 0;
        double zmianaGranicyWPipsach = 1;
        double zmianaGranicyWPipsachHis = 1;

        public string login = "";
        public string password = "";
        
        public Form1()
        {
            InitializeComponent();
            instance = this;
            DataBaseManagment.GetAllTablesNameInDB();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string seconds = DateTime.Now.Second.ToString();
            string minutes = DateTime.Now.Minute.ToString();
            string hours   = DateTime.Now.Hour.ToString();

            if (seconds.Length < 2)
                seconds = '0' + seconds;
            if (minutes.Length < 2)
                minutes = '0' + minutes; 
            if (hours.Length < 2)
                hours = '0' + hours;

            label6.Text = hours + ":" + minutes + ":" + seconds;

            if (Program.bGetNewData == true)
            {
                // Wykonanie obliczeń
                Program.MainProgram(connector, loginResponse);

                // Wypisanie wartości ostatniej świeczki dla danej pary walutowej
                if (true) 
                {
                    label1Symbol0.Text = Trader.Program.symbol[0].ToString();
                    textBox1Symbol0.Text = Program.candlesMatrix[0, 0, 1].ToString();
                    textBox2Symbol0.Text = Program.candlesMatrix[0, 1, 1].ToString();
                    textBox3Symbol0.Text = Program.candlesMatrix[0, 2, 1].ToString();
                    textBox4Symbol0.Text = Program.candlesMatrix[0, 3, 1].ToString();

                    label1Symbol1.Text = Trader.Program.symbol[1].ToString();
                    textBox1Symbol1.Text = Program.candlesMatrix[1, 0, 1].ToString();
                    textBox2Symbol1.Text = Program.candlesMatrix[1, 1, 1].ToString();
                    textBox3Symbol1.Text = Program.candlesMatrix[1, 2, 1].ToString();
                    textBox4Symbol1.Text = Program.candlesMatrix[1, 3, 1].ToString();

                    label1Symbol2.Text = Trader.Program.symbol[2].ToString();
                    textBox1Symbol2.Text = Program.candlesMatrix[2, 0, 1].ToString();
                    textBox2Symbol2.Text = Program.candlesMatrix[2, 1, 1].ToString();
                    textBox3Symbol2.Text = Program.candlesMatrix[2, 2, 1].ToString();
                    textBox4Symbol2.Text = Program.candlesMatrix[2, 3, 1].ToString();

                    label1Symbol3.Text = Trader.Program.symbol[3].ToString();
                    textBox1Symbol3.Text = Program.candlesMatrix[3, 0, 1].ToString();
                    textBox2Symbol3.Text = Program.candlesMatrix[3, 1, 1].ToString();
                    textBox3Symbol3.Text = Program.candlesMatrix[3, 2, 1].ToString();
                    textBox4Symbol3.Text = Program.candlesMatrix[3, 3, 1].ToString();

                    label1Symbol4.Text = Trader.Program.symbol[4].ToString();
                    textBox1Symbol4.Text = Program.candlesMatrix[4, 0, 1].ToString();
                    textBox2Symbol4.Text = Program.candlesMatrix[4, 1, 1].ToString();
                    textBox3Symbol4.Text = Program.candlesMatrix[4, 2, 1].ToString();
                    textBox4Symbol4.Text = Program.candlesMatrix[4, 3, 1].ToString();

                    label1Symbol5.Text = Trader.Program.symbol[5].ToString();
                    textBox1Symbol5.Text = Program.candlesMatrix[5, 0, 1].ToString();
                    textBox2Symbol5.Text = Program.candlesMatrix[5, 1, 1].ToString();
                    textBox3Symbol5.Text = Program.candlesMatrix[5, 2, 1].ToString();
                    textBox4Symbol5.Text = Program.candlesMatrix[5, 3, 1].ToString();

                    label1Symbol6.Text = Trader.Program.symbol[6].ToString();
                    textBox1Symbol6.Text = Program.candlesMatrix[6, 0, 1].ToString();
                    textBox2Symbol6.Text = Program.candlesMatrix[6, 1, 1].ToString();
                    textBox3Symbol6.Text = Program.candlesMatrix[6, 2, 1].ToString();
                    textBox4Symbol6.Text = Program.candlesMatrix[6, 3, 1].ToString();

                    label1Symbol7.Text = Trader.Program.symbol[7].ToString();
                    textBox1Symbol7.Text = Program.candlesMatrix[7, 0, 1].ToString();
                    textBox2Symbol7.Text = Program.candlesMatrix[7, 1, 1].ToString();
                    textBox3Symbol7.Text = Program.candlesMatrix[7, 2, 1].ToString();
                    textBox4Symbol7.Text = Program.candlesMatrix[7, 3, 1].ToString();

                    label1Symbol8.Text = Trader.Program.symbol[8].ToString();
                    textBox1Symbol8.Text = Program.candlesMatrix[8, 0, 1].ToString();
                    textBox2Symbol8.Text = Program.candlesMatrix[8, 1, 1].ToString();
                    textBox3Symbol8.Text = Program.candlesMatrix[8, 2, 1].ToString();
                    textBox4Symbol8.Text = Program.candlesMatrix[8, 3, 1].ToString();
                }
                
                // Aktualizacja danych na wykresie
                UtwórzWykres(selectedSymbol);
            }
        }

        public void Login()
        {
            // Connecting to server
            connector = new SyncAPIConnector(Servers.DEMO);
            loginResponse = APICommandFactory.ExecuteLoginCommand(connector, login, password);
            
            Program.StartProgram(connector);
        }

        private void UtwórzWykres(int n)
        {
            selectedSymbol = n;
            chartMain.Series["Data1"].Points.Clear();
            chartMain.Series["DataSmallSMA"].Points.Clear();
            chartMain.Series["DataBigSMA"].Points.Clear();
            chartMACD.Series["DataUp"].Points.Clear();
            chartMACD.Series["DataDown"].Points.Clear();
            chartMACD.Series["SignalLine"].Points.Clear();

            int howManyCandlesOnChart = 50;
            int ileSwieczek;
            int swieczkaPoczatkowa = 0;
            // Odczytanie wpisanej liczby świeczek przez użytkownika
            if (textBoxPoczatkowaSwieczka.Text != string.Empty)
            {
                swieczkaPoczatkowa = Convert.ToInt32(textBoxPoczatkowaSwieczka.Text);
            }
            if (textBox1IleŚwieczek.Text != string.Empty)
            {
                ileSwieczek = Convert.ToInt32(textBox1IleŚwieczek.Text);
                
                if (ileSwieczek > 0)
                {
                    if (textBoxPoczatkowaSwieczka.Text != string.Empty)
                    {
                        swieczkaPoczatkowa = Convert.ToInt32(textBoxPoczatkowaSwieczka.Text);
                    }

                    if (ileSwieczek + swieczkaPoczatkowa < Program.candleColumns)
                        howManyCandlesOnChart = ileSwieczek;
                }
            }

            // Wypisanie maksymalnej liczby dostępnych świeczek w polu tekstowym
            for (int i = 1; i < Program.candleColumns; i++)
            {
                if (Program.candlesMatrix[n, 0, i] == 0)
                {
                    textBoxMaksSwiec.Text = (i - 1).ToString();
                    break;
                }
                if (i == Program.candleColumns - 1)
                {
                    textBoxMaksSwiec.Text = (i - 1).ToString();
                }
            }

            // Ustawienie tytułu 
            chartMain.Titles["Title1"].Text = "Wykres pary walutowej: " + Program.symbol[n];
            // Ustawienie wykresu świeczek
            if (true) 
            {
                // Ustawienie linii siatki wykresu
                chartMain.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                chartMain.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gainsboro;
                chartMain.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                chartMain.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;

                // Ustawienie osi
                chartMain.ChartAreas[0].AxisX.Interval = 2;
                chartMain.ChartAreas[0].AxisY.LabelStyle.Format = "0.00000";

                // Wpisanie kolejnych punktów na wykresie
                int licznik = 0;
                for (int i = 1 + swieczkaPoczatkowa; i <= howManyCandlesOnChart + swieczkaPoczatkowa; i++)
                {

                    if (Program.candlesMatrix[n, 0, i] == 0)
                    {
                        //howManyCandlesOnChart = 1;
                        break;
                    }
                    chartMain.Series["Data1"].Points.AddXY(licznik, Program.candlesMatrix[n, 2, i],
                        Program.candlesMatrix[n, 1, i], Program.candlesMatrix[n, 0, i], 
                        Program.candlesMatrix[n, 3, i]);
                    licznik--;
                }

                // wyszukanie minimum i maksimum
                if (checkBoxAutoGranice.Checked == true)
                {
                    double min, max;
                    min = Program.candlesMatrix[n, 2, 1 + swieczkaPoczatkowa];
                    max = Program.candlesMatrix[n, 1, 1 + swieczkaPoczatkowa];
                    for (int i = 2 + swieczkaPoczatkowa; i <= howManyCandlesOnChart + swieczkaPoczatkowa; i++)
                    {
                        if (Program.candlesMatrix[n, 2, i] < min && Program.candlesMatrix[n, 2, i] != 0)
                            min = Program.candlesMatrix[n, 2, i];
                        if (Program.candlesMatrix[n, 1, i] > max)
                            max = Program.candlesMatrix[n, 1, i];
                    }

                    chartMain.ChartAreas[0].AxisY.Maximum = max;
                    chartMain.ChartAreas[0].AxisY.Minimum = min;
                }
            }
            // ustawienie wykresu SMA
            if (true)
            {
                int licznik = 0;
                for (int i = 1 + swieczkaPoczatkowa; i < howManyCandlesOnChart + swieczkaPoczatkowa; i++)
                {
                    if(Program.candlesMatrix[n, 9, i]!=0)
                        chartMain.Series["DataSmallSMA"].Points.AddXY(licznik, Program.candlesMatrix[n, 9, i]);
                    if (Program.candlesMatrix[n, 10, i] != 0)
                        chartMain.Series["DataBigSMA"].Points.AddXY(licznik, Program.candlesMatrix[n, 10, i]);
                    licznik--;
                }
            }
            
            // Ustawienie wykresu MACD i linii sygnału
            if(true)
            {
                // Ustawienie linii siatki wykresu
                chartMACD.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                chartMACD.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gainsboro;
                chartMACD.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                chartMACD.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;

                // Ustawienie osi
                chartMACD.ChartAreas[0].AxisX.Interval = 2;
                chartMACD.ChartAreas[0].AxisY.LabelStyle.Format = "0.0000";

                int licznik = 0;
                // Wpisanie kolejnych punktów na wykresie
                for (int i = 1 + swieczkaPoczatkowa; i <= howManyCandlesOnChart + swieczkaPoczatkowa; i++)
                {
                    if (Program.candlesMatrix[n, 0, i] == 0)
                        break;

                    if (Program.candlesMatrix[n, 11, i] > Program.candlesMatrix[n, 12, i])
                        chartMACD.Series["DataUp"].Points.AddXY(licznik, Program.candlesMatrix[n, 11, i]);
                    else
                        chartMACD.Series["DataDown"].Points.AddXY(licznik, Program.candlesMatrix[n, 11, i]);

                    chartMACD.Series["SignalLine"].Points.AddXY(licznik, Program.candlesMatrix[n, 12, i]);

                    licznik--;
                }
            }
            

            // ustawienie wyświetlanego tekstu na przycisku otwórz pozycję
            buttonOtwPozycje.Text = "Otwórz pozycję " + Program.symbol[n];
        }
        private string UtwórzWykresZamkniętejPozycji()
        {
            string zwracanaNazwaUtworzonegoWykresu = "NotClosed";
            string dateTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            chartTrade.Series["Data1"].Points.Clear();
            chartTrade.Series["DataSmallSMA"].Points.Clear();
            chartTrade.Series["DataBigSMA"].Points.Clear();
            chartTrade.Series["Open"].Points.Clear();
            chartTrade.Series["Close"].Points.Clear();

            // Ustawienie tytułu 
            string typTransakcji = "     ";
            if (Program.closedPositionVector[1] == 4)
                typTransakcji = "Buy";
            else if (Program.closedPositionVector[1] == 3)
                typTransakcji = "Sell";
            else
                return zwracanaNazwaUtworzonegoWykresu;
            chartTrade.Titles["Title1"].Text = "Wykres pary walutowej: " +
                Program.symbol[(int)Program.closedPositionVector[0]] + " Typ transakcji: " +  typTransakcji +
                ". Profit: " + Program.closedPositionVector[4] + " Cena otw.: " + Program.closedPositionVector[2] +
                " Cena zamkn.: " + Program.closedPositionVector[3];
            zwracanaNazwaUtworzonegoWykresu = Program.symbol[(int)Program.closedPositionVector[0]] + 
                "_" + typTransakcji +
                "_Profit:" + Program.closedPositionVector[4] + 
                "_Time:" + dateTime;

            // Ustawienie wykresu świeczek
            if (true)
            {
                // Ustawienie linii siatki wykresu
                chartTrade.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                chartTrade.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gainsboro;
                chartTrade.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                chartTrade.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;

                // Ustawienie osi
                chartTrade.ChartAreas[0].AxisX.Interval = 2;
                chartTrade.ChartAreas[0].AxisY.LabelStyle.Format = "0.00000";

                // Wpisanie kolejnych punktów na wykresie
                int licznik = 0;
                for (int i = 1; i < Program.candleColumns; i++)
                {

                    if (Program.closedPositionMatrix[0, i] == 0)
                    {
                        //howManyCandlesOnChart = 1;
                        continue;
                    }
                    chartTrade.Series["Data1"].Points.AddXY(licznik, Program.closedPositionMatrix[2, i],
                        Program.closedPositionMatrix[1, i], Program.closedPositionMatrix[0, i],
                        Program.closedPositionMatrix[3, i]);
                    licznik--;
                }

                // wyszukanie minimum i maksimum
                if (checkBoxAutoGraniceHis.Checked)
                {
                    double min, max;
                    min = Program.closedPositionMatrix[2, 1];
                    max = Program.closedPositionMatrix[1, 1];
                    for (int i = 2; i < Program.candleColumns; i++)
                    {
                        if (Program.closedPositionMatrix[0, i] == 0)
                            break;
                        if (Program.closedPositionMatrix[ 2, i] < min && Program.closedPositionMatrix[2, i] != 0)
                            min = Program.closedPositionMatrix[2, i];
                        if (Program.closedPositionMatrix[1, i] > max)
                            max = Program.closedPositionMatrix[1, i];
                    }

                    chartTrade.ChartAreas[0].AxisY.Maximum = max;
                    chartTrade.ChartAreas[0].AxisY.Minimum = min;
                }
            }
            // ustawienie wykresu SMA
            if (true)
            {
                int licznik = 0;
                for (int i = 1; i < Program.candleColumns; i++)
                {
                    if (Program.closedPositionMatrix[0, i] == 0)
                        break;
                    if (Program.closedPositionMatrix[4, i] != 0)
                        chartTrade.Series["DataSmallSMA"].Points.AddXY(licznik, Program.closedPositionMatrix[4, i]);
                    if (Program.closedPositionMatrix[5, i] != 0)
                        chartTrade.Series["DataBigSMA"].Points.AddXY(licznik, Program.closedPositionMatrix[5, i]);
                    licznik--;
                }
            }
            // ustawienie wykresu otwarcia i zamknięcia pozycji
            if (true)
            {
                for (int i = 0; i < Program.candleColumns; i++)
                {
                    if (Program.closedPositionMatrix[8, i] != 0)
                        chartTrade.Series["Open"].Points.AddXY(-i, Program.closedPositionMatrix[8, i]);
                    if (Program.closedPositionMatrix[9, i] != 0)
                        chartTrade.Series["Close"].Points.AddXY(-i, Program.closedPositionMatrix[9, i]);
                }
            }

            // Wyczyszczenie danych z vectora zamkniętej pozycji


            return zwracanaNazwaUtworzonegoWykresu;
        }

        // Utwórz wykres dla danej pozycji przyciski
        private void button1pobierzWykres0_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(0);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(1);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(2);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres3_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(3);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres4_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(4);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres5_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(5);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres6_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(6);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres7_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(7);

            Cursor.Current = Cursors.Default;
        }
        private void button1pobierzWykres8_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(8);

            Cursor.Current = Cursors.Default;
        }

        // Obsługa zamknięcia okna
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Czy na pewno chcesz wyjść z aplikacji?", "Zamknij aplikację", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        // Zmiana zakresu wyliczanego SMA, zmiana liczby wyświetlanych świec
        private void ChangeSMASize_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            int newBigSMA;
            int newSmallSMA;

            if (textBoxBigSMA.Text != string.Empty)
                newBigSMA = Convert.ToInt32(textBoxBigSMA.Text);
            else
                newBigSMA = Program.bigSMA;

            if (textBoxSmallSMA.Text != string.Empty)
                newSmallSMA = Convert.ToInt32(textBoxSmallSMA.Text);
            else
                newSmallSMA = Program.smallSMA;


            if (textBoxSmallSMA.Text != string.Empty || textBoxBigSMA.Text != string.Empty)
            { 
                // jeżeli small SMA jest większe od big SMA to zamień ich wartości
                if (newSmallSMA > newBigSMA)
                {
                    int x = newBigSMA;
                    newBigSMA = newSmallSMA;
                    newSmallSMA = newBigSMA;
                }

                if (newSmallSMA > 0)
                {
                    if (newSmallSMA != Program.smallSMA)
                    {
                        // wyczyszczenie starego wskaźnika
                        for (int i = 0; i < Program.symbol.Length; i++)
                        {
                            for (int j = 0; j < Program.candleColumns; j++)
                            {
                                Program.candlesMatrix[i, 9, j] = 0;
                            }
                        }
                        
                        
                        Program.smallSMA = newSmallSMA;
                        label15.Text = newSmallSMA.ToString();
                    }
                }


                if (newBigSMA > 0)
                {
                    if (newBigSMA != Program.bigSMA)
                    {
                        // wyczyszczenie starego wskaźnika
                        for (int i = 0; i < Program.symbol.Length; i++)
                        {
                            for (int j = 0; j < Program.candleColumns; j++)
                            {
                                Program.candlesMatrix[i, 10, j] = 0;
                            }
                        }

                        Program.bigSMA = newBigSMA;
                        label16.Text = newBigSMA.ToString();
                    }
                }
                

                if (textBoxSmallSMA.Text != string.Empty || textBoxBigSMA.Text != string.Empty)
                {
                    // Reupload Data
                    Program.Disconnect(connector);
                    Login();
                }
            }

            Cursor.Current = Cursors.Default;
        }
        private void buttonZmienIloscWyswietlanychSwieczek_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            UtwórzWykres(selectedSymbol);

            Cursor.Current = Cursors.Default;
        }

        // Różne tryby działania aplikacji
        private void buttonModeStreaming_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Wyłączenie przycisków związanych z trybem pracy na historycznych danych
            panelTrybHistory.Enabled = false;

            // Wielkość macierzy dla trybu streaming
            Program.candleColumns = 600;
            Program.candlesMatrix = new double[Program.symbol.Length, Program.candleRows, Program.candleColumns];
            // Utworzenie nowego połaczenia
            Program.Disconnect(connector);
            Login();

            // Włączenie przycisków do otwierania i zamykania pozycji
            panelOtwartePozycje.Enabled = true;
            panelZamknietePozycje.Enabled = true;

            // Reset ustawionych zmiennych odpowiadających za to która świeczka jest początkowa
            // i ile wyświetlanych jest na wykresie
            textBoxPoczatkowaSwieczka.Text = 0.ToString();
            textBox1IleŚwieczek.Text = 50.ToString();

            Cursor.Current = Cursors.Default;
        }
        private void buttonModeHistory_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Włączenie przycisków związanych z trybem pracy na historycznych danych
            panelTrybHistory.Enabled = true;

            Program.Disconnect(connector);
            // Connecting to server
            connector = new SyncAPIConnector(Servers.DEMO);
            loginResponse = APICommandFactory.ExecuteLoginCommand(connector, login, password);

            // Wielkość macierzy dla trybu danych historycznych
            Program.candleColumns = 50000;
            Program.candlesMatrix = new double[Program.symbol.Length, Program.candleRows, Program.candleColumns];

            // Wyłączenie przycisków do otwierania i zamykania pozycji
            panelOtwartePozycje.Enabled = false;
            panelZamknietePozycje.Enabled = false;

            // Pobranie listy dostępnych danych historycznych
            DataBaseManagment.GetAllHistoricalTablesNameInDB();

            UtwórzWykres(selectedSymbol);

            Cursor.Current = Cursors.Default;
        }


        // Zmiana granic wykresu głównego
        private void buttonMaksPlus_Click(object sender, EventArgs e)
        {
            chartMain.ChartAreas[0].AxisY.Maximum += zmianaGranicyWPipsach / 10000;
        }
        private void buttonMaksMinus_Click(object sender, EventArgs e)
        {
            if(chartMain.ChartAreas[0].AxisY.Maximum - zmianaGranicyWPipsach / 10000 > chartMain.ChartAreas[0].AxisY.Minimum)
            chartMain.ChartAreas[0].AxisY.Maximum -= zmianaGranicyWPipsach / 10000;
        }
        private void buttonMinPlus_Click(object sender, EventArgs e)
        {
            if(chartMain.ChartAreas[0].AxisY.Minimum + zmianaGranicyWPipsach / 10000 < chartMain.ChartAreas[0].AxisY.Maximum)
                chartMain.ChartAreas[0].AxisY.Minimum += zmianaGranicyWPipsach / 10000;
        }
        private void buttonMinMinus_Click(object sender, EventArgs e)
        {
            chartMain.ChartAreas[0].AxisY.Minimum -= zmianaGranicyWPipsach / 10000;
        }
        private void textBoxZmianaGranicWykresu_TextChanged(object sender, EventArgs e)
        {
            if (textBoxZmianaGranicWykresu.Text != string.Empty)
            {
                zmianaGranicyWPipsach = Convert.ToDouble(textBoxZmianaGranicWykresu.Text);
            }
        }
        
        // Zmiana granic wykresu historycznego pozycji zamkniętych
        private void buttonMaksPlusHist_Click(object sender, EventArgs e)
        {
            chartTrade.ChartAreas[0].AxisY.Maximum += zmianaGranicyWPipsachHis / 10000;
        }
        private void buttonMkasMinusHis_Click(object sender, EventArgs e)
        {
            if (chartTrade.ChartAreas[0].AxisY.Maximum - zmianaGranicyWPipsachHis / 10000 > chartTrade.ChartAreas[0].AxisY.Minimum)
                chartTrade.ChartAreas[0].AxisY.Maximum -= zmianaGranicyWPipsachHis / 10000;
        }
        private void buttonMinPlusHis_Click(object sender, EventArgs e)
        {
            if (chartTrade.ChartAreas[0].AxisY.Minimum + zmianaGranicyWPipsachHis / 10000 < chartTrade.ChartAreas[0].AxisY.Maximum)
                chartTrade.ChartAreas[0].AxisY.Minimum += zmianaGranicyWPipsachHis / 10000;
        }
        private void buttonMinMinusHis_Click(object sender, EventArgs e)
        {
            chartTrade.ChartAreas[0].AxisY.Minimum -= zmianaGranicyWPipsachHis / 10000;
        }
        private void textBoxZmianaGranicWykresuHistorycznego_TextChanged(object sender, EventArgs e)
        {
            if (textBoxZmianaGranicWykresuHistorycznego.Text != string.Empty)
            {
                zmianaGranicyWPipsachHis = Convert.ToDouble(textBoxZmianaGranicWykresuHistorycznego.Text);
            }
        }

        // Obsługa pozycji przyciskami - otwórz, zamknij, usuń
        private void buttonOtwPozycje_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Ustawienie tytułu zapisywanej pozycji w liście 
            string dateTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            string typTransakcji;

            if (checkBoxOpenSellPosition.Checked)
            {
                Program.OpenTradeSell(connector, selectedSymbol, ref Program.candlesMatrix);
                
                typTransakcji = "Sell";
                string nazwaPozycjiNaLiscie = Program.symbol[selectedSymbol] +
                    "_" + typTransakcji +
                    "_Time:" + dateTime;

                addItemToOpenPosList(nazwaPozycjiNaLiscie);
            }
            else if (checkBoxOpenBuyPosition.Checked)
            {
                Program.OpenTradeBuy(connector, selectedSymbol, ref Program.candlesMatrix);

                typTransakcji = "Buy";
                string nazwaPozycjiNaLiscie = Program.symbol[selectedSymbol] +
                    "_" + typTransakcji +
                    "_Time:" + dateTime;

                addItemToOpenPosList(nazwaPozycjiNaLiscie);
            }
            else 
            {
                MessageBox.Show("Nie wybrano typu otwieranej pozycji (buy/sell)!");
            }

            Cursor.Current = Cursors.Default;
        }
        private void buttonZamkPozycje_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (listBoxOtwartePozycje.SelectedIndex >= 0)
            {
                // znalezienie indexu symbolu zaznaczonej pozycji
                string nazwaOtwartejPozycji = listBoxOtwartePozycje.Items[listBoxOtwartePozycje.SelectedIndex].ToString();
                string symbolOtwartejPozycji = nazwaOtwartejPozycji.Substring(0, 6);
                int indexSymboluOtwartejPozycji = 0;
                for (int i = 1; i < Program.symbol.Length; i++)
                {
                    if (symbolOtwartejPozycji == Program.symbol[i])
                    {
                        indexSymboluOtwartejPozycji = i;
                        break;
                    }

                }

                // Znalezienie typu zaznaczonej pozycji
                string typOtwartejPozycji = nazwaOtwartejPozycji.Substring(7, 1);

                if (typOtwartejPozycji == "S")
                {
                    //Zamknięcie pozycji
                    Program.CloseLastOpenedSellTrade(connector, indexSymboluOtwartejPozycji, ref Program.candlesMatrix);
                }
                if (typOtwartejPozycji == "B")
                {
                    //Zamknięcie pozycji
                    Program.CloseLastOpenedBuyTrade(connector, indexSymboluOtwartejPozycji, ref Program.candlesMatrix);
                }


                // Zapisanie danych do wykresu w bazie danych i utworzenie pozycji w liście
                string nazwaTabeli = UtwórzWykresZamkniętejPozycji();

                if (nazwaTabeli != "NotClosed")
                {
                    listBoxZamknietePozycje.Items.Add(nazwaTabeli);

                    DataBaseManagment.CreateTable(nazwaTabeli);
                    DataBaseManagment.InsertIntoTable(nazwaTabeli);
                }
                else
                {
                    MessageBox.Show("Pozycja została już zamknięta!");
                }
                // Usunięcie zamkniętej pozycji z listy otwartych pozycji
                listBoxOtwartePozycje.Items.Remove(listBoxOtwartePozycje.Items[listBoxOtwartePozycje.SelectedIndex]);

            }
            else
            {
                MessageBox.Show("Nie wybrano otwartej pozycji, która ma być zamknięta!");
            }

            Cursor.Current = Cursors.Default;
        }
        private void buttonUsunPozycje_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (listBoxZamknietePozycje.SelectedIndex >= 0) // wybrano jakiś index
            {
                string nazwaTabeli = listBoxZamknietePozycje.Items[listBoxZamknietePozycje.SelectedIndex].ToString();
                DataBaseManagment.DeleteTable(nazwaTabeli);
                listBoxZamknietePozycje.Items.Remove(
                    listBoxZamknietePozycje.Items[listBoxZamknietePozycje.SelectedIndex]);
            }
            else
            {

                MessageBox.Show("Nie wybrano zamkniętej pozycji, która ma być usunięta!");
            }

            Cursor.Current = Cursors.Default;
        }
        private void buttonRetrieve_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string nazwaTabeli = listBoxZamknietePozycje.Items[listBoxZamknietePozycje.SelectedIndex].ToString();
            DataBaseManagment.RetrieveDataFromTable(nazwaTabeli);
            UtwórzWykresZamkniętejPozycji();

            Cursor.Current = Cursors.Default;
        }
        public  void addItemToOpenPosList(string nazwaPozycjiNaLiscie)
        {
            foreach(var item in listBoxOtwartePozycje.Items)
            {
                if (item.ToString() == nazwaPozycjiNaLiscie)
                    return;
            }
            listBoxOtwartePozycje.Items.Add(nazwaPozycjiNaLiscie);
        }
        public  void addToSavedClosedPositionList(string nazwaTabeli)
        {
            // Pominięcie nazw tabeli z dopiskiem Name (tabele zawierające kolumny używane do tytułu generowanego wykresu)
            if (nazwaTabeli.Substring(nazwaTabeli.Length - 4) != "Name")
            {
                // Dodanie pozycji do listy 
                listBoxZamknietePozycje.Items.Add(nazwaTabeli);
            }
        }
        public  void addToHistoricalDataList(string nazwaTabeli)
        {
            // Wyszukiwanie tylko tabel z początkiem nazwy EURUSD
            if (nazwaTabeli.Substring(0, 6) != "EURUSD")
                return;

            // Pominięcie 7 pierwszych znaków zawierających symbol
            nazwaTabeli = nazwaTabeli.Substring(nazwaTabeli.Length - (nazwaTabeli.Length - 7));

            // Jeżeli lista zawiera już taką pozycję to pomiń
            foreach (var item in listBoxHistoricalData.Items)
            {
               if (item.ToString() == nazwaTabeli)
                    return;
            }

            // Dodanie pozycji do listy 
            listBoxHistoricalData.Items.Add(nazwaTabeli);
            
        }
        private void checkBoxOpenBuyPosition_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOpenBuyPosition.Checked)
                checkBoxOpenSellPosition.Checked = false;
        }
        private void checkBoxOpenSellPosition_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOpenSellPosition.Checked)
                checkBoxOpenBuyPosition.Checked = false;
        }

        // Pobieranie, zapisywanie, usuwanie i odzyskiwanie danych z tabeli
        private void buttonGetCurrentData_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            Program.GetFiftyThousandCandlesHistory(connector, Program.candleColumns);

            Cursor.Current = Cursors.Default;
        }
        private void buttonSaveCurrentData_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            DataBaseManagment.CreateHistoricalCandlesTable();

            Cursor.Current = Cursors.Default;
        }
        private void buttonDeleteCurrentData_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            for (int n = 0; n < Program.symbol.Length; n++)
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

                tableName = Program.symbol[n] + "_" + date.Year + "_" + month + "_" + day;

                DataBaseManagment.DeleteHistoricalCandlesTable(tableName);
            }

            Cursor.Current = Cursors.Default;
        }
        private void buttonRetrieveData_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

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

            for (int n = 0; n < Program.symbol.Length; n++)
            {

                //tableName = Program.symbol[n] + "_" + date.Year + "_" + month + "_" + day;
                tableName = Program.symbol[n] + "_" + textBoxRetrieveData.Text;

                DataBaseManagment.RetrieveDataFromHistoricalCandlesTable(tableName, n);
            }

            UtwórzWykres(selectedSymbol);

            Cursor.Current = Cursors.Default;
        }





        

        private void buttonAnalyseData_Click(object sender, EventArgs e)
        {
            labelProfit.Text = "Profit: " + Program.MakeTradeOnHistoricalData(connector).ToString();
        }


        private void listBoxHistoricalData_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxRetrieveData.Text = 
                listBoxHistoricalData.SelectedItem.ToString();
        }
    }

}