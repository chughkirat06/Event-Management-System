using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupProjectTest
{
    /// <summary>
    /// Interaction logic for Buy.xaml
    /// </summary>
    public partial class Buy : Window
    {
        public Buy()
        {
            InitializeComponent();
        }

        private void GoToViewDetails(object sender, RoutedEventArgs e)
        {
            string eventname = EventName.Text;
            ViewEvent_Admin a = new ViewEvent_Admin();
            a.ViewEvent_Load(eventname);
            a.Show();
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

        private void HomeClick(object sender, MouseButtonEventArgs e)
        {
            AdminDashboard dashboard = new AdminDashboard();
            dashboard.Show();
            this.Close();
        }

        private void MyTicketsClick(object sender, RoutedEventArgs e)
        {
            MyTickets ticketlist = new MyTickets();
            ticketlist.TicketsTable(Global.userId);
            ticketlist.Show();
            this.Close();
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

        public void PopulateData(string Eventname)
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
                    EventId.Text = reader["EventId"].ToString();
                    EventName.Text = reader["EventName"].ToString();
                    EventType.Text = reader["EventType"].ToString();
                    DateTime fromDate = Convert.ToDateTime(reader["EventFromDate"]);
                    DateTime toDate = Convert.ToDateTime(reader["EventToDate"]);

                    EventFromTime.Text = reader["EventFromTime"].ToString() + "    -";
                    EventToTime.Text = reader["EventToTime"].ToString();
                    EventVenue.Text = reader["EventVenue"].ToString();

                    DateTime EventFromDate = fromDate;
                    DateTime EventToDate = toDate;


                    for (DateTime date = EventFromDate; date <= EventToDate; date = date.AddDays(1))
                    {
                        DateTime currentDate = DateTime.Now;
                        if (date >= currentDate.Date)
                        {
                            SelectDate.Items.Add(date.ToString("yyyy-MM-dd"));
                        }
                    }



                    int totalTickets = int.Parse(reader["TicketQuantity"].ToString());


                    // check if the event is sold out
                    if (totalTickets == 0 || SelectDate.Items.Count == 0)
                    {
                        // hide the user details section
                        userDetailSection.Visibility = Visibility.Hidden;
                        // hide buttons
                        buttonSection.Visibility = Visibility.Hidden;
                        // display message
                        soldOutMessage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // hide sold message
                        soldOutMessage.Visibility= Visibility.Hidden;

                        // Set total on page load
                         decimal ticketPrice = decimal.Parse(reader["TicketPrice"].ToString());
                         int purchasedTickets = int.Parse(PurchasedTickets.Text);
                         decimal total = ticketPrice * purchasedTickets;
                         TicketPrice.Text = "$" + total.ToString("0.00");
                    }
                }
                reader.Close();
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

        private void TicketQuantity(object sender, TextChangedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "SELECT * FROM Eventtb WHERE EventName = @EventName";

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlcmd.Parameters.AddWithValue("@EventName", EventName.Text);
                SqlDataReader reader = sqlcmd.ExecuteReader();

                while (reader.Read())
                {
                    if (int.TryParse(PurchasedTickets.Text, out int enteredTickets) && int.TryParse(reader["TicketQuantity"].ToString(), out int maxTickets))
                    {
                        if (enteredTickets > maxTickets)
                        {
                            MessageBox.Show("Maximum available tickets are " + maxTickets , "Sorry!", MessageBoxButton.OK, MessageBoxImage.Stop);
                            PurchasedTickets.Text = "";
                            return;
                        }
                        else if (enteredTickets == 0)
                        {
                            MessageBox.Show("Please enter ticket quantity greater than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            PurchasedTickets.Text = "";
                            return;
                        }
                    }

                    // Set total after entering tickets quantity
                    decimal ticketPrice = decimal.Parse(reader["TicketPrice"].ToString());
                    int purchasedTickets = 0;

                    if (string.IsNullOrEmpty(PurchasedTickets.Text))
                    {
                        //TicketPrice.Text = "$" + ticketPrice.ToString();
                        TicketPrice.Text = "$ 0.00";
                        return;
                    }
                    else
                    {
                        purchasedTickets = int.Parse(PurchasedTickets.Text);
                        decimal total = ticketPrice * purchasedTickets;
                        TicketPrice.Text = "$" + total.ToString("0.00");
                    }
                }
                reader.Close();
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


        private void ApplyPromoCode(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;");
            try
            {
                String query = "SELECT * FROM Eventtb WHERE EventName = @EventName";

                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlcmd.Parameters.AddWithValue("@EventName", EventName.Text);
                SqlDataReader reader = sqlcmd.ExecuteReader();

                while (reader.Read())
                {
                    string promoCodeDB = reader["PromoCode"].ToString();
                    decimal ticketPrice;
                    int purchasedTickets;

                    if (!decimal.TryParse(reader["TicketPrice"].ToString(), out ticketPrice))
                    {
                        //MessageBox.Show("Invalid ticket price format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!int.TryParse(PurchasedTickets.Text, out purchasedTickets))
                    {
                        //MessageBox.Show("Invalid purchased tickets format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(promoCodeDB))
                    {
                        return;
                    }

                    if (PromoCode.Text == promoCodeDB)
                    {
                        if (string.IsNullOrEmpty(PromoCode.Text))
                        {
                            return;
                        }
                        else
                        {
                            // Apply 10% discount
                            decimal discountedPrice = ticketPrice * 0.9m;
                            decimal total = discountedPrice * purchasedTickets;
                            TicketPrice.Text = "$" + total.ToString("0.00");
                            MessageBox.Show("Coupon applied successfully!!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(PromoCode.Text))
                        {
                            return;
                        }
                        else
                        {
                            decimal total = ticketPrice * purchasedTickets;
                            TicketPrice.Text = "$" + total.ToString("0.00");
                            MessageBox.Show("Invalid coupon!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            PromoCode.Text = "";
                            return;
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlCon.Close();
            }
        }




        private void Cancel_AddEventForm(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you don't want to buy tickets?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                AdminDashboard dash = new AdminDashboard();
                dash.Show();
                this.Close();
            }
        }

        private void Back_Event(object sender, RoutedEventArgs e)
        {
            AdminDashboard dash = new AdminDashboard();
            dash.Show();
            this.Close();
        }

        private void PurchaseTicket(object sender, RoutedEventArgs e)
        {
            // Check if input is not empty and quantity is greater than 0
            if (string.IsNullOrEmpty(PurchasedTickets.Text))
            {
                MessageBox.Show("Please enter ticket quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectDate.Text))
            {
                MessageBox.Show("Please select the date", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            using (SqlConnection sqlCon = new SqlConnection(@"Data Source=LAPTOP-CHDNPOHB\SQLEXPRESS; Initial Catalog=LoginDB; Integrated Security = True;"))
            {
                String query = "SELECT * FROM Eventtb WHERE EventName = @EventName";
                SqlCommand sqlcmd = new SqlCommand(query, sqlCon);
                sqlCon.Open();
                sqlcmd.Parameters.AddWithValue("@EventName", EventName.Text);


                using (SqlDataReader reader = sqlcmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (int.TryParse(PurchasedTickets.Text, out int enteredTickets) && int.TryParse(reader["TicketQuantity"].ToString(), out int maxTickets))
                        {
                            reader.Close();
                            try
                            {
                                // Update ticket quantity in database
                                int remainingTickets = maxTickets - enteredTickets;
                                String updateQuery = "UPDATE Eventtb SET TicketQuantity = @RemainingTickets WHERE EventName = @EventName";
                                SqlCommand updateCmd = new SqlCommand(updateQuery, sqlCon);
                                updateCmd.Parameters.AddWithValue("@EventName", EventName.Text);
                                updateCmd.Parameters.AddWithValue("@RemainingTickets", remainingTickets);
                                updateCmd.ExecuteNonQuery();

                                String Buyquery = "INSERT INTO Bookingtb (BookingId, UserId, EventId, SelectedDate, PurchasedTickets, TotalPrice)";

                                Buyquery += "VALUES (@BookingId, @UserId, @EventId, @SelectedDate, @PurchasedTickets, @TotalPrice)";

                                SqlCommand buycmd = new SqlCommand(Buyquery, sqlCon);
                                string bookingId = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                                buycmd.Parameters.AddWithValue("@BookingId", bookingId);
                                buycmd.Parameters.AddWithValue("@UserId", Global.userId);
                                buycmd.Parameters.AddWithValue("@EventId", EventId.Text);
                                buycmd.Parameters.AddWithValue("@SelectedDate", SelectDate.Text);
                                buycmd.Parameters.AddWithValue("@PurchasedTickets", int.Parse(PurchasedTickets.Text));
                                buycmd.Parameters.AddWithValue("@TotalPrice", TicketPrice.Text);
                                buycmd.ExecuteNonQuery();

                                // Navigate to congratulations page
                                BookingSuccessful done = new BookingSuccessful();
                                done.BookingSuccess_Load(bookingId);
                                done.Show();
                                this.Close();

                                MessageBox.Show("Congratulations !! Booking Successful!!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                    }
                    reader.Close();
                }
            }
        }
    }
}

