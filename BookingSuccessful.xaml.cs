using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for BookingSuccessful.xaml
    /// </summary>
    public partial class BookingSuccessful : Window
    {
        public BookingSuccessful()
        {
            InitializeComponent();
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

        private void HomeClick(object sender, MouseButtonEventArgs e)
        {
            AdminDashboard dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

        private void AccountSettings(object sender, RoutedEventArgs e)
        {
            AccountSettings account = new AccountSettings();
            account.Show();
            this.Close();
        }

        public void BookingSuccess_Load(string bookingId)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "SELECT b.*, e.EventVenue, e.EventType, e.EventFromTime, e.EventToTime, e.EventName FROM Bookingtb b JOIN Eventtb e ON b.EventId = e.EventId WHERE b.BookingId = @BookingId";
                

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlcmd.Parameters.AddWithValue("@BookingId", bookingId);
                SqlDataReader reader = sqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    BookingId.Content = "Booking ID  #" +reader["BookingId"].ToString();
                    EventName.Content = reader["EventName"].ToString() + "(" +reader["EventType"].ToString() +")";
                    DateTime selectedDate = Convert.ToDateTime(reader["SelectedDate"]);
                    Date.Content = selectedDate.ToShortDateString() + "  |  ";

                    TimeFrom.Content = reader["EventFromTime"].ToString() + "  -  ";
                    TimeTo.Content = reader["EventToTime"].ToString();
                    Venue.Content = reader["EventVenue"].ToString();
                    PurchasedTickets.Content = reader["PurchasedTickets"].ToString() + " Ticket(s)";
                    TotalPrice.Content = reader["TotalPrice"].ToString();
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

        private void CloseButton_ViewEvent(object sender, RoutedEventArgs e)
        {
            MyTickets ticketlist = new MyTickets();
            ticketlist.TicketsTable(Global.userId);
            ticketlist.Show();
            this.Close();
        }
    }
}
