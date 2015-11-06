using Facebook.Client;
using ResKueMe.Constant;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ResKueMe.Facebook
{
    public class FacebookUtil
    {
        private static FacebookSession _facebookSession;
        private static IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        private static readonly object _sync = new object();
        public static async void Authenticate()
        {
            string message = String.Empty;             
            try
            {
                Console.Write("I am being called ");

                _facebookSession = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream");
                //IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"] = _facebookSession.AccessToken;
                //IsolatedStorageSettings.ApplicationSettings["FacebookId"] = _facebookSession.FacebookId;
                appSettings.Add("FacebookAccessToken", _facebookSession.AccessToken);
                appSettings.Add("FacebookId", _facebookSession.FacebookId);
                //SaveSettings();
               // IsolatedStorageSettings.ApplicationSettings.Save();
                App.RootFrame.Navigate(new Uri("/ContactsView.xaml", UriKind.Relative));
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show("Login failed! Exception details: " + e.Message);
                 
            }             
            
        }
        private static void SaveSettings()
        {
            lock (_sync)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
    }
}
