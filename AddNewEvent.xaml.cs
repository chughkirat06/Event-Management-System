using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for AddNewEvent.xaml
    /// </summary>
    public partial class AddNewEvent : Window
    {
        public AddNewEvent()
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


        private void Cancel_AddEventForm(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure that you would like to close the form?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                AdminDashboard dash = new AdminDashboard();
                dash.Show();
                this.Close();
            }
        }

        private void AddEventtoDB(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "INSERT INTO Eventtb (EventName , EventType, EventDescription, EventFromDate, EventToDate, EventFromTime, EventToTime, EventVenue, TicketPrice, TicketQuantity, PromoCode, TermsnCondition)";

                query += "VALUES (@EventName, @EventType, @EventDescription, @EventFromDate, @EventToDate, @EventFromTime, @EventToTime, @EventVenue, @TicketPrice, @TicketQuantity, @PromoCode, @TermsnCondition)";

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);

                if(string.IsNullOrEmpty(EventName.Text) || string.IsNullOrEmpty(EventType.Text) || string.IsNullOrEmpty(EventFromDate.Text) || string.IsNullOrEmpty(EventToDate.Text) || string.IsNullOrEmpty(EventFromTime.Text) || string.IsNullOrEmpty(EventToTime.Text) || string.IsNullOrEmpty(EventVenue.Text)|| string.IsNullOrEmpty(TicketPrice.Text) || string.IsNullOrEmpty(TicketQuantity.Text))
                {
                    MessageBox.Show("Please fill all mandatory fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (EventToDate.SelectedDate.Value.Date < EventFromDate.SelectedDate.Value.Date)
                {
                    MessageBox.Show("From Date cannot be greater than To Date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    sqlcmd.Parameters.AddWithValue("@EventName", EventName.Text);
                    sqlcmd.Parameters.AddWithValue("@EventType", EventType.Text);
                    sqlcmd.Parameters.AddWithValue("@EventDescription", EventDescription.Text);
                    sqlcmd.Parameters.AddWithValue("@EventFromDate", EventFromDate.SelectedDate.Value.Date.ToShortDateString());
                    sqlcmd.Parameters.AddWithValue("@EventToDate", EventToDate.SelectedDate.Value.Date.ToShortDateString());
                    sqlcmd.Parameters.AddWithValue("@EventFromTime", EventFromTime.Text);
                    sqlcmd.Parameters.AddWithValue("@EventToTime", EventToTime.Text);
                    sqlcmd.Parameters.AddWithValue("@EventVenue", EventVenue.Text);
                    sqlcmd.Parameters.AddWithValue("@TicketPrice", TicketPrice.Text);
                    sqlcmd.Parameters.AddWithValue("@TicketQuantity", TicketQuantity.Text);
                    sqlcmd.Parameters.AddWithValue("@PromoCode", PromoCode.Text);
                    sqlcmd.Parameters.AddWithValue("@TermsnCondition", TermsnConditions.Text);
                    
                    sqlCon.Open();

                    sqlcmd.ExecuteNonQuery();
                    MessageBox.Show("Event Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void FieldAcceptingOnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
