using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for ViewEvent_Admin.xaml
    /// </summary>
    public partial class ViewEvent_Admin : Window
    {
        public ViewEvent_Admin()
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

        public void ViewEvent_Load( string Eventname)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "SELECT * FROM Eventtb WHERE EventName = @EventName";

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlcmd.Parameters.AddWithValue("@EventName", Eventname);
                SqlDataReader reader = sqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    EventName.Text = reader["EventName"].ToString();
                    EventType.Text = reader["EventType"].ToString();
                    EventDescription.Text = reader["EventDescription"].ToString();

                    DateTime fromDate = Convert.ToDateTime(reader["EventFromDate"]);
                    EventFromDate.Text = fromDate.ToShortDateString() + "    -";
                    
                    DateTime toDate = Convert.ToDateTime(reader["EventToDate"]);
                    EventToDate.Text = toDate.ToShortDateString();

                    EventFromTime.Text = reader["EventFromTime"].ToString() + "      -";
                    EventToTime.Text = reader["EventToTime"].ToString();
                    EventVenue.Text = reader["EventVenue"].ToString();
                    TicketPrice.Text = reader["TicketPrice"].ToString();
                    TicketQuantity.Text = reader["TicketQuantity"].ToString();
                    PromoCode.Text = reader["PromoCode"].ToString();
                    if (Global.userId != 1)
                    {
                        TicketAvailable.Visibility = Visibility.Collapsed;
                        TicketQuantity.Visibility= Visibility.Collapsed;
                        PromoCodeLabel.Visibility = Visibility.Collapsed;
                        PromoCode.Visibility = Visibility.Collapsed;

                        TicketPrice.Margin = new Thickness(-450, 0, 0, -702);
                        TicketQuantity.Margin = new Thickness(150, 0, 0, -702);
                    }
                    TermsnConditions.Text = reader["TermsnCondition"].ToString();
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

        private void BuyTicket(object sender, RoutedEventArgs e)
        {
            string eventname = EventName.Text;
            Buy BuyPage = new Buy();
            BuyPage.PopulateData(eventname);
            BuyPage.Show();
            this.Close();
        }

        private void CloseButton_ViewEvent(object sender, RoutedEventArgs e)
        {
                AdminDashboard dash = new AdminDashboard();
                dash.Show();
                this.Close();
        }
    }
}
