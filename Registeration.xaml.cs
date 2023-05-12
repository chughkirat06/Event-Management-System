using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;


namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for Registeration.xaml
    /// </summary>
    public partial class Registeration : Window
    {
        public Registeration()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "INSERT INTO Usertb (UserName , Email, Password)";

                query += "VALUES (@UserName, @Email, @Password)";

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Password) || string.IsNullOrEmpty(txtRePassword.Password))
                {
                    MessageBox.Show("Please fill all mandatory fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if(EmailExistsInDatabase(txtEmail.Text))
                {
                    MessageBox.Show("This email already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (txtPassword.Password != txtRePassword.Password)
                {
                    MessageBox.Show("Passwords doesnot match! Please retry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    sqlcmd.Parameters.AddWithValue("@UserName", txtUsername.Text);
                    sqlcmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    sqlcmd.Parameters.AddWithValue("@Password", txtPassword.Password);

                    sqlCon.Open();

                    sqlcmd.ExecuteNonQuery();
                    MessageBox.Show("Registration Successfull", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoginScreen l = new LoginScreen();
                    l.Show();
                    this.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private bool EmailExistsInDatabase(string email)
        {
            bool emailExists = false;
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            String query = "SELECT COUNT(*) FROM Usertb WHERE Email = @Email";
            SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
            sqlcmd.Parameters.AddWithValue("@Email", email);
            sqlCon.Open();

            int count = (int)sqlcmd.ExecuteScalar();
            if (count > 0)
            {
                emailExists = true;
            }

            sqlCon.Close();
            return emailExists;
        }
        private void GoToLoginPage(object sender, MouseButtonEventArgs e)
        {
            LoginScreen l = new LoginScreen();
            l.Show();
            this.Close();
        }
    }
}
