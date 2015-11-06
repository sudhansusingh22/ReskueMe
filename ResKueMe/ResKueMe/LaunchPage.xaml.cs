using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using ResKueMe.Facebook;
using System.Threading;
using ResKueMe.Tools;
using Microsoft.Phone.Net.NetworkInformation;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Tasks;
using ResKueMe.GeoLocation;

namespace ResKueMe
{
    public partial class LaunchPage : PhoneApplicationPage
    {
        
        public LaunchPage()
        {
            InitializeComponent();
            
            var taskCheckForLocationService = Task.Run(() => LocAndInternetCheck.CheckForLocationService());
            taskCheckForLocationService.Wait();
            var result = true;
            //taskCheckForLocationService.Result.Result; 
            //
            if (result)
            {
                // Go to Login url
                try
                {
                    // Clear Cookie to remove current logged in user data
                    mWebBrowser.ClearCookiesAsync();
                    mWebBrowser.Source = new Uri(FacebookClients.Instance.GetLoginUrl());
                }
                catch (System.Net.WebException ex)
                {
                    MessageBox.Show("Internet connection is not working : " + ex.Message);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error while Facebook Login : " + ex.Message);
                }
            }
        }

        void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("Application is terminating as it can't work without location service.");
            });
        }

        

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                String uri = e.Uri.ToString();

                if (uri.StartsWith("https://m.facebook.com/connect/login_success.html") || uri.StartsWith("http://m.facebook.com/connect/login_success.html"))
                {
                    // Remove junk text added by facebook from url
                    if (uri.EndsWith("#_=_"))
                        uri = uri.Substring(0, uri.Length - 4);

                    String queryString = e.Uri.Query.ToString();

                    // Acquire the code from Query String
                    IEnumerable<KeyValuePair<string, string>> pairs = UriToolKits.ParseQueryString(queryString);
                    string code = KeyValuePairUtils.GetValue(pairs, "code");

                    // Get access_token from code using Asynchronous HTTP Request
                    WebClient client = new WebClient();
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(AccessTokenDownloadCompleted);
                    client.DownloadStringAsync(new Uri(FacebookClients.Instance.GetAccessTokenRequestUrl(code)));
                }
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error in Connecting to facebook : " + ex.Message);
            }
        }
        void AccessTokenDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string data = e.Result;
                data = "?" + data;

                // Acquire access_token and expires timestamp
                IEnumerable<KeyValuePair<string, string>> pairs = UriToolKits.ParseQueryString(data);
                string accessToken = KeyValuePairUtils.GetValue(pairs, "access_token");
                string expires = KeyValuePairUtils.GetValue(pairs, "expires");

                // Save access_token
                FacebookClients.Instance.AccessToken = accessToken;

                // Back to MainPage
                // var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                //if (rootFrame != null)
                //  rootFrame.GoBack();
                App.RootFrame.Navigate(new Uri("/ContactsView.xaml", UriKind.Relative));
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while getting access token : " + ex.Message);
            }

        }
      /*  protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("FacebookAccessToken"))
            {
                string facebookToken = IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"] as string;
                if (facebookToken == null)
                {
                     Deployment.Current.Dispatcher.BeginInvoke(() => FacebookUtil.Authenticate());                   
                }

            }
            else
            {

                Deployment.Current.Dispatcher.BeginInvoke(() => FacebookUtil.Authenticate());    
            }
            
            
        }*/
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Exiting the app");
            base.OnBackKeyPress(e);
        }
    }
}