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
using System.Collections.Generic;

namespace ResKueMe.Controls
{
    public class LongListMultiSelector : Microsoft.Phone.Controls.LongListMultiSelector
    {
         public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof (object), typeof (LongListMultiSelector), new PropertyMetadata(default(object)));
 
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof (SelectionMode), typeof (LongListMultiSelector), new PropertyMetadata(default(SelectionMode)));
 
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode) GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
 
        public  object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
 
        public LongListMultiSelector()
        {
            SelectionMode = SelectionMode.Single;
 
            SelectionChanged += (sender, args) =>
            {
                if(SelectionMode == SelectionMode.Single)
                    SelectedItem = args.AddedItems[0];
                else if (SelectionMode == SelectionMode.Multiple)
                {
                    if (SelectedItem == null)
                    {
                        SelectedItem = new List<object>();
                    }
 
                    foreach (var item in args.AddedItems)
                    {
                        ((List<object>)SelectedItem).Add(item);                        
                    }
 
                    foreach (var removedItem in args.RemovedItems)
                    {
                        if (((List<object>) SelectedItem).Contains(removedItem))
                        {
                            ((List<object>) SelectedItem).Remove(removedItem);
                        }
                    }
                }
            };
        }
    }
    }

