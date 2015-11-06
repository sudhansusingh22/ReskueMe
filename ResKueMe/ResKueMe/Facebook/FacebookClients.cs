using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using ResKueMe.Constant;
using System.Windows;

namespace ResKueMe.Facebook
{
    public class FacebookClients
    {
        private static FacebookClients instance;
        private string accessToken;

        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        private String appId = Constants.FacebookAppId;
        private String clientSecret = Constants.FacebookClientSecret;
        private String scope = "publish_stream";

        public FacebookClients()
        {
            try
            {
                accessToken = (string)appSettings["accessToken"];
            }
            catch (KeyNotFoundException e)
            {
                accessToken = "";
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error while connecting to facebook : " + ex.Message);
            }
        }

        public static FacebookClients Instance
        {
            get
            {
                if (instance == null)
                    instance = new FacebookClients();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public string AccessToken
        {
            get
            {
                return accessToken;
            }
            set
            {
                accessToken = value;
                if (accessToken.Equals(""))
                    appSettings.Remove("accessToken");
                else
                    appSettings.Add("accessToken", accessToken);
            }
        }

        public virtual String GetLoginUrl()
        {
            try
            {
                return "https://m.facebook.com/dialog/oauth?client_id=" + appId + "&redirect_uri=https://m.facebook.com/connect/login_success.html&scope=" + scope + "&display=touch";

            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
                return "";
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error while connecting to facebook : " + ex.Message);
                return "";
            }

        }

        public virtual String GetAccessTokenRequestUrl(string code)
        {
            try
            {
                return "https://graph.facebook.com/oauth/access_token?client_id=" + appId + "&redirect_uri=https://m.facebook.com/connect/login_success.html&client_secret=" + clientSecret + "&code=" + code;
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
                return "";
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error while connecting to facebook : " + ex.Message);
                return "";
            }
        }

        public virtual String GetAccessTokenExchangeUrl(string accessToken)
        {
            try
            {
                return "https://graph.facebook.com/oauth/access_token?client_id=" + appId + "&client_secret=" + clientSecret + "&grant_type=fb_exchange_token&fb_exchange_token=" + accessToken;

            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
                return "";
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error while connecting to facebook : " + ex.Message);
                return "";
            }
        }

        public void PostMessageOnWall(string message, UploadStringCompletedEventHandler handler)
        {
            try
            {
                WebClient client = new WebClient();
                client.UploadStringCompleted += handler;
                client.UploadStringAsync(new Uri("https://graph.facebook.com/me/feed"), "POST", "message=" + HttpUtility.UrlEncode(message) + "&access_token=" + FacebookClients.Instance.AccessToken);

            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show("Internet connection is not working : " + ex.Message);
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error while posting status on facebook : " + ex.Message);
            }
        }

        public void ExchangeAccessToken(UploadStringCompletedEventHandler handler)
        {
            try
            {
                WebClient client = new WebClient();
                client.UploadStringCompleted += handler;
                client.UploadStringAsync(new Uri(GetAccessTokenExchangeUrl(FacebookClients.Instance.AccessToken)), "POST", "");

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

    }
}
