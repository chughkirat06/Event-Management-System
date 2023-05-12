using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for MyTickets.xaml
    /// </summary>
    public partial class MyTickets : Window
    {
        public MyTickets()
        {
            InitializeComponent();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If search box is empty, display all events
                TicketsTable(Global.userId);
            }
            else
            {
                SearchButton_Click(sender, e);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If search box is empty, display all events
                TicketsTable(Global.userId);
                return;
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security=True;"))
                {
                    SqlCommand command = new SqlCommand("SELECT b.BookingId, e.EventName, b.PurchasedTickets, CONVERT(varchar(10), b.SelectedDate, 101) as EventDate FROM Bookingtb b JOIN Eventtb e ON b.EventId = e.EventId WHERE (e.EventName LIKE @searchTerm OR b.BookingId LIKE @searchTerm) AND b.UserId = @userId", connection);
                    command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    command.Parameters.AddWithValue("@userId", Global.userId);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        // Bind the result set to a list or grid to display the matching events.
                        EventData.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("No events found.");
                        SearchBox.Text = String.Empty;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void TicketsTable(int userId)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "SELECT b.BookingId, e.EventName, b.PurchasedTickets, CONVERT(varchar(10), b.SelectedDate, 101) as EventDate FROM Bookingtb b JOIN Eventtb e ON b.EventId = e.EventId WHERE b.UserId = @UserId";

                sqlCon.Open();
                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlcmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                EventData.ItemsSource = dt.DefaultView;
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


        public void ViewTicketDetails(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)e.Source).DataContext;
            string id = row["BookingId"].ToString();
            BookingSuccessful done = new BookingSuccessful();
            done.BookingSuccess_Load(id);
            done.Show();
            this.Close();
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

        private void AccountSettings(object sender, RoutedEventArgs e)
        {
            AccountSettings account = new AccountSettings();
            account.Show();
            this.Close();
        }

        private void HomeClick(object sender, MouseButtonEventArgs e)
        {
            AdminDashboard dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

        private void Log_Out_Click(object sender, MouseButtonEventArgs e)
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
    }
}
