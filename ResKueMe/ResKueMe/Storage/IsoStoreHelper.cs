using ResKueMe.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ResKueMe.Storage
{
    public class IsoStoreHelper
    {
        public  IsolatedStorageFile _isoStore;
        public  IsolatedStorageFile IsoStore
        {
            get { return _isoStore ?? (_isoStore = IsolatedStorageFile.GetUserStoreForApplication()); }
        }

        public void SaveList<T>(string folderName, string dataName, List<T> dataList) where T : class
        {
            if (!IsoStore.DirectoryExists(folderName))
            {
                IsoStore.CreateDirectory(folderName);
            }

            string fileStreamName = string.Format("{0}\\{1}.dat", folderName, dataName);

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileStreamName, FileMode.Create, IsoStore))
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(List<T>));
                dcs.WriteObject(stream, dataList);
            }
        }

        public List<T> LoadList<T>(string folderName, string dataName) where T : class
        {
            List<T> retval = new List<T>();

            if (!IsoStore.DirectoryExists(folderName))
            {
                IsoStore.CreateDirectory(folderName);
            }

            string fileStreamName = string.Format("{0}\\{1}.dat", folderName, dataName);

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileStreamName, FileMode.OpenOrCreate, IsoStore))
            {
                if (stream.Length > 0)
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<T>));
                    retval = dcs.ReadObject(stream) as List<T>;
                }
            }

            return retval;
        }
        public void deleteList<T>(string folderName, string fileName) where T : class
        {
            IsoStore.DeleteFile(fileName);
            IsoStore.DeleteDirectory(folderName);
        }
    }
}
