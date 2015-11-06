using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace ResKueMe.GeoLocation
{
    public class LocAndInternetCheck
    {
        public static async Task<bool> CheckForLocationService()
        {
            // A message box would be shown if the loaction  and internet is off.

            var success = true;

            bool checkFlag = false;
            bool locationFlag = false;
            bool internetFlag = false;
            String errorMessage = "Following items needs immediate attention:\n(Application needs to be relaunched after making changes.)\n";
            bool isNetwork = NetworkInterface.GetIsNetworkAvailable();            
            Geolocator geolocator = new Geolocator();        
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.MovementThreshold = 100;
 
            if (geolocator.LocationStatus == PositionStatus.Disabled)
            {              
                success = false;
                errorMessage += "\u25C9 Please turn on location by clicking on OK button. \n\n";
                checkFlag = true;
                locationFlag = true;
            }
            if (isNetwork == false)
            {
                errorMessage += "\u25C9 Please turn on internet connection:\n     \u2023 Either connect a wifi \n     \u2023 Or turn on mobile connectivity.";
                checkFlag = true;
                internetFlag = true;
            }
            if (checkFlag == true)
            {

                // Only location is off
                if (locationFlag == true)
                {
  
                    Deployment.Current.Dispatcher.BeginInvoke(async () =>  
                    {                         
                            MessageBoxResult messageBoxResult = MessageBox.Show(errorMessage, "\u274F Attention", MessageBoxButton.OKCancel);
                            if (messageBoxResult == MessageBoxResult.OK)
                            {
                                string uriToLaunch = "ms-settings-location:";
                                // Create a Uri object from a URI string 
                                var uri = new Uri(uriToLaunch);
                                success = await Windows.System.Launcher.LaunchUriAsync(uri);
                                Application.Current.Terminate();

                            }
                            else if (messageBoxResult == MessageBoxResult.Cancel)
                            {
                                MessageBox.Show("Application is terminating as it can't work without location service.");
                                Application.Current.Terminate();
                            }
                         
                    });
                     
                }
                // only internet is off
                else if (internetFlag == true)
                {

                    Deployment.Current.Dispatcher.BeginInvoke(async () =>
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show(errorMessage, "\u274F Attention", MessageBoxButton.OKCancel);
                        if (messageBoxResult == MessageBoxResult.OK)
                        {
                            string uriToLaunch = "ms-settings-cellular:";
                            // Create a Uri object from a URI string 
                            var uri = new Uri(uriToLaunch);
                            success = await Windows.System.Launcher.LaunchUriAsync(uri);
                            Application.Current.Terminate();

                        }
                        else if (messageBoxResult == MessageBoxResult.Cancel)
                        {
                            MessageBox.Show("Application is terminating as it can't work without internet service.");
                            Application.Current.Terminate();
                        }

                    });

                }
                // both location and internet is off
                else if (internetFlag == true && locationFlag == true)
                {

                    Deployment.Current.Dispatcher.BeginInvoke(async () =>
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show(errorMessage, "\u274F Attention", MessageBoxButton.OKCancel);
                        if (messageBoxResult == MessageBoxResult.OK)
                        {
                            string uriToLaunch = "ms-settings-location:";
                            // Create a Uri object from a URI string 
                            var uri = new Uri(uriToLaunch);
                            success = await Windows.System.Launcher.LaunchUriAsync(uri);
                            Application.Current.Terminate();

                        }
                        else if (messageBoxResult == MessageBoxResult.Cancel)
                        {
                            MessageBox.Show("Application is terminating as it can't work without location service.");
                            Application.Current.Terminate();
                        }

                    });

                }

            }
            return success;
        }
       
    }
}
