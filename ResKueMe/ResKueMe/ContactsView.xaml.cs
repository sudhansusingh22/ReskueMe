using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ResKueMe
{
    public partial class ContactsView : PhoneApplicationPage
    {
        public ContactsView()
        {
            try
            {
                ResKueProgressIndicator("Loading Contacts...");
                SetProgressIndicator(true);
                this.DataContext = new ContactsViewModel();
                InitializeComponent();
                SetProgressIndicator(false);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while loading contact page : " + ex.Message);
            }

        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void OnPivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {

        }
        // Progress Indicator Codes
        public void ResKueProgressIndicator(String displayText)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();
           SystemTray.ProgressIndicator.Text = displayText;
        }
        private void SetProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
           SystemTray.ProgressIndicator.IsVisible = value;
        }
        // Privacy Policy
        private void PrivacyPolicyCommand(object sender, EventArgs e)
        {
            App.RootFrame.Navigate(new Uri("/PrivacyPolicy.xaml", UriKind.Relative));
        }
    }

}