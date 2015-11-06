using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ResKueMe.Model
{
    public class SavedContact
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }

    [DataContract]
    public class ResKueContact : INotifyPropertyChanged
    {
        public ResKueContact() { }
        private bool _isSelected;
        private Contact _contact;

        [DataMember]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }

            }
        }

        [DataMember]
        public Contact Contact
        {
            get
            {
                return _contact;
            }
            set
            {
                if (_contact != value)
                {
                    _contact = value;
                    OnPropertyChanged("Contact");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
