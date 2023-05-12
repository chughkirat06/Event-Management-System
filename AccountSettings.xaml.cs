using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for AccountSettings.xaml
    /// </summary>
    public partial class AccountSettings : Window
    {
        public AccountSettings()
        {
            InitializeComponent();
            AccountSettings_Load();
        }

        private void ContactUs(object sender, RoutedEventArgs e)
        {
            Contact_Us contact = new Contact_Us();
            contact.Show();
            this.Close();

        }

        private void MyTicketsClick(object sender, RoutedEventArgs e)
        {
            MyTickets ticketlist = new MyTickets();
            ticketlist.TicketsTable(Global.userId);
            ticketlist.Show();
            this.Close();
        }

        private void AccountSetting(object sender, RoutedEventArgs e)
        {
            AccountSettings account = new AccountSettings();
            account.Show();
            this.Close();

        }

        private void HomeClick(object sender, MouseButtonEventArgs e )
        {
            AdminDashboard dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();

        }

        private void Log_Out_Click(object sender, MouseButtonEventArgs e )
        {
            var result = MessageBox.Show("Are you sure?", "LogOut", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                LoginScreen logIn = new LoginScreen();
                logIn.Show();
                this.Close();
            }
        }

        private void Account_Dropdown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var image = sender as Image;
                if (image != null)
                {
                    image.ContextMenu.IsOpen = true;
                }
            }
        }

        public void AccountSettings_Load()
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {


                string username = Global.userName;

                String query = "SELECT * FROM Usertb WHERE UserName = @UserName";

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlcmd.Parameters.AddWithValue("@UserName", username);
                sqlCon.Open();

                SqlDataReader reader = sqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    UserId.Text = reader["UserId"].ToString();
                    Name.Text = reader["UserName"].ToString();
                    Global.email = Email.Text = reader["Email"].ToString();
                    Password.Password = reader["Password"].ToString();
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

        private void Cancel_Form(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure that you would like to cancel?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                AdminDashboard dash = new AdminDashboard();
                dash.Show();
                this.Close();
            }



        }

        private void UpdateUserSettings(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                int Id = int.Parse(UserId.Text);
                String query = "UPDATE Usertb SET UserName = @UserName, Email = @Email, Password = @Password WHERE UserId = @UserId";                

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);

                if (string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(Email.Text) || string.IsNullOrEmpty(Password.Password) )
                {
                    MessageBox.Show("Please fill all mandatory fields", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (EmailExistsInDatabase(Email.Text))
                {
                    MessageBox.Show("This email already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    sqlcmd.Parameters.AddWithValue("@UserId", Id);
                    sqlcmd.Parameters.AddWithValue("@UserName", Name.Text);
                    sqlcmd.Parameters.AddWithValue("@Email", Email.Text);
                    sqlcmd.Parameters.AddWithValue("@Password", Password.Password);


                    sqlCon.Open();

                    sqlcmd.ExecuteNonQuery();
                    Global.userName = Name.Text;
                    MessageBox.Show("Account Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    AdminDashboard dash = new AdminDashboard();
                    dash.Show();
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
            string newEmail = Email.Text;
            string currentEmail = Global.email;

            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");

            String query = "SELECT COUNT(*) as count FROM Usertb WHERE Email=@NewEmail AND Email <> @CurrentUserEmail";
            SqlCommand sqlcmd = new SqlCommand(query, sqlCon);

            sqlcmd.Parameters.AddWithValue("@NewEmail", newEmail);
            sqlcmd.Parameters.AddWithValue("@CurrentUserEmail", currentEmail);

            sqlCon.Open();

            int count = (int)sqlcmd.ExecuteScalar();
            if (count > 0 )
            {
                emailExists = true;
            }

            sqlCon.Close();
            return emailExists;
        }

       
    }
}
