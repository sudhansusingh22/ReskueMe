using ResKueMe.Model;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ResKueMe.UriMapper
{
    public class LoginUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            if (uri.OriginalString == "/LaunchPage.xaml")
            {
                 IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                 if (appSettings.Contains("accessToken"))
                 {
                     var facebookAccessToken = IsolatedStorageSettings.ApplicationSettings["accessToken"] as string;
                     if (facebookAccessToken == null)
                     {
                         uri = new Uri("/LaunchPage.xaml", UriKind.Relative);
                     }                     
                     if (appSettings.Contains("SavedContacts"))
                     {
                         var savedContacts = IsolatedStorageSettings.ApplicationSettings["SavedContacts"] as List<SavedContact>;
                         if (savedContacts == null)
                         {
                             uri = new Uri("/ContactsView.xaml", UriKind.Relative);
                         }
                         else
                         {
                             uri = new Uri("/MainPage.xaml", UriKind.Relative);
                         }
                     }
                     else
                     {
                         uri = new Uri("/LaunchPage.xaml", UriKind.Relative);
                     }
                 }
                 else
                 {
                     uri = new Uri("/LaunchPage.xaml", UriKind.Relative);
                 }
                
            }
            return uri;
        }
    }
}
