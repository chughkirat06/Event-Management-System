using System;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;


namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void SubmitButtn_Click(object sender, RoutedEventArgs e)
        {
            
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;"); 
            try
            {
                String query = "SELECT COUNT(*) as count, UserName, UserId FROM Usertb WHERE Email=@Email AND Password COLLATE SQL_Latin1_General_CP1_CS_AS=@Password GROUP BY UserName, UserId";
                SqlCommand  sqlcmd = new SqlCommand(query, sqlCon);

                if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Password))
                {
                    MessageBox.Show("Please fill all mandatory fields", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    sqlcmd.CommandType = System.Data.CommandType.Text;
                    sqlcmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    sqlcmd.Parameters.AddWithValue("@Password", txtPassword.Password);
                    
                    sqlCon.Open();

                    SqlDataReader reader = sqlcmd.ExecuteReader();


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int count = Convert.ToInt32(reader["count"]);
                            if (count == 1)
                            {
                                Global.userName = reader["UserName"].ToString();
                                Global.userId = int.Parse(reader["UserId"].ToString());

                                AdminDashboard dashboard = new AdminDashboard();
                                dashboard.Show();

                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email or Password is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void GoToRegisterationPage(object sender, MouseButtonEventArgs e)
        {
            Registeration r = new Registeration();
            r.Show();
            this.Close();
        }
    }
}



