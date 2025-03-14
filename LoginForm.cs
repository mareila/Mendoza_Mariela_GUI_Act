using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Mendoza_Mariela_Act_GUI
{
    public partial class LoginForm : Form
    {
        private string validUsername = "Mariela";
        private string validPassword = "marielamaganda";
        private int loginAttempts = 0;

        private const string serverName = "localhost";
        private const string dataBaseName = "StudentInfoDB";
        private const string uid = "root";
        private const string pssword = "0000";

        private int index = 0;
        private DataTable dt = new DataTable();

        public string usrname;

        public MySqlConnection con { get; set; }
        public MySqlCommand cmd { get; set; }
        public MySqlDataReader dr { get; set; }

        public LoginForm()
        {
            con = new MySqlConnection($"Server={serverName};Database={dataBaseName};Uid={uid};Pwd={pssword};");
            cmd = new MySqlCommand();
            cmd.Connection = this.con;
            InitializeComponent();
            if (!Connect())
                MessageBox.Show("Please Configure Your Connection");
        }

        public bool Connect()
        {
            if (this.con.State == ConnectionState.Closed || this.con.State == ConnectionState.Broken)
            {
                try
                {
                    this.con.Open();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public void Disconnect()
        {
            if (this.con.State == ConnectionState.Open)
            {
                this.con.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                load_users();
            }
            else
            {
                Application.Exit();
            }
        }

        private void load_users()
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select * from USERLOGINTB";
            dr = cmd.ExecuteReader();
            dt.Clear();
            dt.Load(dr);
            dr.Close();
        }

        private void usercred(int i)
        {
            usrname = dt.Rows[i][1].ToString();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            loginAttempts++;
            if (loginAttempts > 5)
            {
                MessageBox.Show("Too many failed login attempts. Please reset your password using the link below.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show("Reset password Link: www.example.com/resetpassword", "Reset password Link", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                return;
            }

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            bool loginSuccessful = false;

            foreach (DataRow row in dt.Rows)
            {
                if (row["username"].ToString() == username && row["password"].ToString() == password)
                {
                    loginSuccessful = true;
                    break;
                }
            }

            if (loginSuccessful)
            {
                MessageBox.Show("Login Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                StudentForm studentForm = new StudentForm();
                this.Hide();
                studentForm.Show();
            }
            else
            {
                string errorMessage = "";
                if (!dt.AsEnumerable().Any(row => row["username"].ToString() == username))
                {
                    errorMessage += "Incorrect Username. ";
                }
                if (!dt.AsEnumerable().Any(row => row["password"].ToString() == password))
                {
                    errorMessage += "Incorrect Password. ";
                }
                MessageBox.Show(errorMessage + $"\nAttempt login: {loginAttempts} of 5", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PasswordTextBox.Clear();
                PasswordTextBox.Focus();
            }
        }

        private void PasswordLabel_Click(object sender, EventArgs e)
        {
        }
    }
}