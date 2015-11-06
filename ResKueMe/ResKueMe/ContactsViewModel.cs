using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;
using ResKueMe.Model;
using ResKueMe.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ResKueMe
{
    public class ContactsViewModel : ViewModelBase
    {
        private static IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        private List<ResKueContact> _contactList;

        public ContactsViewModel()
        {
            try
            {
                Contacts cons = new Contacts();
                cons.SearchAsync(String.Empty, FilterKind.None, null);
                cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(OnContactSearchCompleted);
                SaveResKueContactsCommand = new RelayCommand(OnSaveResKueContacts);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while getting phone contact information : " + ex.Message);
            }
        }

        public RelayCommand SaveResKueContactsCommand { get; private set; }

        private void OnSaveResKueContacts()
        {
            try
            {
                if (!ResKueContactsList.Any(x => x.IsSelected == true))
                {
                    MessageBox.Show("Please select some contacts and then click on Save button.");
                }
                else
                {
                    var selectedSavedContacts = ResKueContactsList.Where(x => x.IsSelected == true).Select(x => new SavedContact { Name = x.Contact.DisplayName, PhoneNumber = x.Contact.PhoneNumbers.ElementAt(0).PhoneNumber }).ToList();
                    ReskueSavedContacts = selectedSavedContacts;
                    MessageBox.Show("Emergency contact list added successfully.");

                    App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error while saving contact information : " + ex.Message);
            }
        }

        void OnContactSearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            try
            {
                ResKueContactsList = new List<ResKueContact>();
                var allContacts = new List<Contact>(e.Results.Where(x => x.PhoneNumbers.Count() > 0).OrderBy(c => c.DisplayName));

                foreach (Contact contact in allContacts)
                {
                    ResKueContact SavedContact = new ResKueContact() { Contact = contact };
                    if (ReskueSavedContacts.Any(x => x.PhoneNumber == contact.PhoneNumbers.ElementAt(0).PhoneNumber))
                    {
                        SavedContact.IsSelected = true;
                    }
                    else
                    {
                        SavedContact.IsSelected = false;
                    }
                    ResKueContactsList.Add(SavedContact);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error in retrieving contacts : " + ex.Message);
            }

        }

        [DataMember]
        public List<ResKueContact> ResKueContactsList
        {
            get { return _contactList; }
            set
            {
                _contactList = value;
                RaisePropertyChanged("ResKueContactsList");
            }
        }

        public List<SavedContact> ReskueSavedContacts
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
