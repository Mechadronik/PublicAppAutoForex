using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trader
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }


        // Login to trading platform, show main program window, close formLogin
        // after press login button or enter key 
        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            await LoginAndShowMainAppWindowAsync();
        }
        private async void FormLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            await LoginAndShowMainAppWindowAsync();
        }
        private async void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await LoginAndShowMainAppWindowAsync();
            }
        }
        private async void textBoxLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await LoginAndShowMainAppWindowAsync();
            }
        }
        private async Task LoginAndShowMainAppWindowAsync()
        {
            try
            {
                panel1.Visible = false;

                Form1 f1 = new Form1();
                f1.login = textBoxLogin.Text;
                f1.password = textBoxPassword.Text;

                await Task.Run(() => f1.Login());
                this.Hide();
                f1.ShowDialog();
                this.Close();
            }
            catch
            {
                panel1.Visible = true;
                labelWrongLoginOrPassword.Visible = true;
            }
        }

        // Display messageBox when user closes window
        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to close the application?", "Close application"
                    , MessageBoxButtons.OKCancel);

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

        // Display info when Caps Lock key is ON
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                labelCapsLockOnPassword.Visible = true;
            }
            else
            {
                labelCapsLockOnPassword.Visible = false;
            }
        }

        // Hide info about wrong login or password, when user changes them
        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
            labelWrongLoginOrPassword.Visible = false;
        }
        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            labelWrongLoginOrPassword.Visible = false;
        }


    }
}
