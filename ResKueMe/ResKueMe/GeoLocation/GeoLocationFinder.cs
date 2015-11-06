using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace ResKueMe.GeoLocation
{
    public class GeoLocationFinder
    {
        public static string Latitude;
        public static string Longitude;

        public async static void FindMyLocation()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.Default;
            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                Latitude = geoposition.Coordinate.Latitude.ToString();
                Longitude = geoposition.Coordinate.Longitude.ToString();
            }

            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(()=>MessageBox.Show("Error in finding location " + ex.Message));
            }
             
        }            
       
    }
}
