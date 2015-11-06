using Facebook;
using Facebook.Client;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.UserData;
using ResKueMe.Facebook;
using ResKueMe.GeoLocation;
using ResKueMe.Model;
using ResKueMe.SMS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ResKueMe
{
    public class MainPageViewModel : ViewModelBase
    {
        //private readonly string _facebookId;
        private readonly string _facebookToken;
        private static IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;        
        public static string _longitude;
        public static string _latitude;
         
        public MainPageViewModel()
        {
            try
            {
                GetCoordinate();
                RaiseAlarmCommand = new RelayCommand(OnRaiseAlarmCommand);
                ChooseContactsCommand = new RelayCommand(OnChooseContactsCommand);
                //_facebookId = IsolatedStorageSettings.ApplicationSettings["FacebookId"] as string;
                //_facebookToken = IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"] as string; 
                //_facebookId = (string)appSettings["FacebookId"];
                _facebookToken = (string)appSettings["accessToken"];
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error while gettng access token in main page : " + ex.Message);
            }

        }
         
        private void GetCoordinate()
        {
            try
            {
                var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                {
                    MovementThreshold = 1
                };
                watcher.PositionChanged += this.watcher_PositionChanged;
                watcher.Start();
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while getting Location information in main page : " + ex.Message);
            }
        }
        
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            try
            {
                var pos = e.Position.Location;
                _latitude = pos.Latitude.ToString("0.000");
                _longitude = pos.Longitude.ToString("0.000");
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while getting Location information in main page : " + ex.Message);
            }

        }
         

        public RelayCommand ChooseContactsCommand { get; private set; }
        public RelayCommand RaiseAlarmCommand { get; private set; }
    
        
        private void OnChooseContactsCommand()
        {
            try
            {
                App.RootFrame.Navigate(new Uri("/ContactsView.xaml", UriKind.Relative));
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while getting contacts information in main page : " + ex.Message);
            }
        }
        private void OnRaiseAlarmCommand()
        {
            try
            {
                FindMyLocation(_facebookToken);
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while finding location : " + ex.Message);
            }
        }
        
        private void FindMyLocation(string accessToken)
        {
            try
            {
                Task taskFindLocation = Task.Factory.StartNew(() => GeoLocationFinder.FindMyLocation());
                taskFindLocation.Wait();

                var a = App.Current as App;
                var fbClient = new FacebookClient(accessToken);
                fbClient.GetCompleted += new EventHandler<FacebookApiEventArgs>(onMessageSentCompletion);
                Dictionary<string, object> searchParams = new Dictionary<string, object>();

                searchParams.Add("center", _latitude + "," + _longitude);
                searchParams.Add("type", "place");
                searchParams.Add("distance", "1000");
                //fbClient.GetAsync("/search", searchParams);
                fbClient.GetTaskAsync("/search", searchParams);
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while posting status in facebook : " + ex.Message);
            }
        }
        private void onMessageSentCompletion(object sender, FacebookApiEventArgs e)
        {
            try
            {
                var a = App.Current as App;
                if (e.Error == null)
                {
                    var result = (IDictionary<string, object>)e.GetResultData();
                    var model = new List<Place>();
                    foreach (var place in (JsonArray)result["data"])
                        model.Add(new Place()
                        {
                            Id = (string)(((JsonObject)place)["id"]),
                            Name = (string)(((JsonObject)place)["name"])
                        });

                    FacebookClient fb = new FacebookClient(_facebookToken);

                    var param = new Dictionary<string, object> { { "message", "I am in danger at :" }, 
                                                            { "place", model.FirstOrDefault().Id },
                                                            {"acess_token",_facebookToken},
                                                            {"privacy", new Dictionary<string, object>{{"value", "ALL_FRIENDS"}}}};
                    // Posting status on facebook
                    fb.PostTaskAsync("me/feed", param, new System.Threading.CancellationToken());

                    // Calling  method to send sms
                    SMSUtils smsmUtil = new SMSUtils();
                    smsmUtil.SendText(GetSavedNumbers(), "I am in danger at :" + model.FirstOrDefault().Name);



                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Error ocurred in sending messages " + e.Error.Message);
                    });
                }
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while posting status in facebook : " + ex.Message);
            }

        }
        private static List<string> GetSavedNumbers()
        {
            try
            {
                List<string> numbers = new List<string>();
                foreach (SavedContact contact in ReskueSavedContacts)
                {
                    numbers.Add(contact.PhoneNumber);
                }
                return numbers;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while getting contacts information in main page : " + ex.Message);
                return null;
            }
        }
        public static List<SavedContact> ReskueSavedContacts
        {
            get
            {
                if (appSettings.Contains("SavedContacts"))
                {
                    var savedContacts = (List<SavedContact>)IsolatedStorageSettings.ApplicationSettings["SavedContacts"];
                    return savedContacts;
                }
                else
                {
                    return new List<SavedContact>();
                }
            }
            set
            {
                if (value != null)
                {
                    IsolatedStorageSettings.ApplicationSettings["SavedContacts"] = value;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }
    }
}
