using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ResKueMe.Resources;
using System.Threading.Tasks;
using Facebook;
using Facebook.Client;
using Windows.Devices.Geolocation;
using ResKueMe.Facebook;
using ResKueMe.GeoLocation;
using Microsoft.Phone.Net.NetworkInformation;

namespace ResKueMe
{
    public partial class MainPage : PhoneApplicationPage
    {     
        public MainPage()
        {
            // calling showLocMessageBox to show location and internet connectivity alert
            try
            {
                var taskCheckForLocationService = Task.Run(() => LocAndInternetCheck.CheckForLocationService());
                taskCheckForLocationService.Wait();
           
                this.DataContext = new MainPageViewModel();
                InitializeComponent();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while loading main Page : " + ex.Message);
            }
        }
        // Exit the app on click of back button
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Exiting the app");
            base.OnBackKeyPress(e);
        }
       
    }
}