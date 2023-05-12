using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        private void Dashboard_Load()
        {
            WelcomeMsg.Text = "Welcome back, " + Global.userName  + "!";
            if (Global.userId != 1)
            {
                    AddEventButton.Visibility = Visibility.Collapsed;                    
            }
        }

        public AdminDashboard()
        {
            InitializeComponent();
            EventTable();
            Dashboard_Load();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If search box is empty, display all events
                EventTable();
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
                EventTable();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;"))
                {
                    SqlCommand command = new SqlCommand("SELECT EventID, EventName, EventType, CONVERT(varchar(10), EventFromDate, 101) as EventDate FROM Eventtb WHERE EventName LIKE @searchTerm OR EventType LIKE @searchTerm", connection);
                    command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

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


        public void EventTable()
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "SELECT EventID, EventName, EventType, CONVERT(varchar(10), EventFromDate, 101) as EventDate  FROM Eventtb";
                sqlCon.Open();
                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                DataTable dt= new DataTable();
                da.Fill(dt);

                EventData.ItemsSource = dt.DefaultView;

                if(Global.userId != 1)
                {
                    EventData.Columns[1].Visibility = Visibility.Hidden;
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

        private void ContactUs(object sender, RoutedEventArgs e)
        {
                Contact_Us contact = new Contact_Us();
                contact.Show();
                this.Close();
            
        }

        private void AccountSettings(object sender, RoutedEventArgs e)
        {
            AccountSettings account = new AccountSettings();
            account.Show();
            this.Close();
        }

        private void MyTicketsClick(object sender, RoutedEventArgs e)
        {
            MyTickets ticketlist= new MyTickets();
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

        private void CreateNewEventForm(object sender, RoutedEventArgs e)
        {
            AddNewEvent EventForm = new AddNewEvent();
            EventForm.Show();
            this.Close();
        }

        private void ViewEventForm(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)e.Source).DataContext;
            //string eventid = row["EventID"].ToString();
            string eventname = row["EventName"].ToString();
            ViewEvent_Admin a = new ViewEvent_Admin();
            a.ViewEvent_Load(eventname);
            a.Show();
            this.Close();
        }

        private void UpdateEventForm(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)e.Source).DataContext;
            string eventname = row["EventName"].ToString();
            UpdateEvent UpdateForm = new UpdateEvent();
            UpdateForm.UpdateEvent_Load(eventname);
            UpdateForm.Show();
            this.Close();
        }

        private void Buy_Page(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)e.Source).DataContext;
            string eventname = row["EventName"].ToString();
            Buy BuyPage = new Buy();
            BuyPage.PopulateData(eventname);
            BuyPage.Show();
            this.Close();
        }

        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                DataRowView row = (DataRowView)((Button)e.Source).DataContext;
                string eventname = row["EventName"].ToString();
                string eventid = row["EventID"].ToString();

                var result = MessageBox.Show("Are you sure you want to delete this event?", "Delete Event", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // delete all associated bookings first
                    String query1 = "DELETE Bookingtb WHERE EventId = @EventId";
                    SqlCommand sqlcmd1 = new SqlCommand(query1, sqlCon);
                    sqlcmd1.Parameters.AddWithValue("@EventId", eventid);
                    sqlCon.Open();
                    sqlcmd1.ExecuteNonQuery();
                    sqlCon.Close();

                    // delete the event itself
                    String query2 = "DELETE Eventtb WHERE EventID = @EventID";
                    SqlCommand sqlcmd2 = new SqlCommand(query2, sqlCon);
                    sqlcmd2.Parameters.AddWithValue("@EventID", eventid);
                    sqlCon.Open();
                    sqlcmd2.ExecuteNonQuery();
                    sqlCon.Close();

                    EventTable();
                    MessageBox.Show("Event deleted successfully", "Delete Success", MessageBoxButton.OK, MessageBoxImage.Information); 
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

    }
}
