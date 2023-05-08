using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public class DataService : INotifyPropertyChanged
    {
        private static DataService instance;

        public static DataService Instance => instance ??= new DataService();
        public ObservableStringCollection Types => new ObservableStringCollection(Properties.Settings.Default.Types);

        ObservableCollection<string> typesNames = new ObservableCollection<string>();
        public ObservableCollection<string> TypesNames
        {
            get
            {
                typesNames.Clear();
                foreach (string type in Types)
                {
                    typesNames.Add(type);
                }
                foreach (var type in Entries)
                {
                    typesNames.Add(type);
                }
                return typesNames;
            }
        }


        public void AddType(string type)
        {
            Types.Add(type);
            Properties.Settings.Default.Save();
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(TypesNames));
        }

        public void RemoveType(string type)
        {
            Types.Remove(type);
            Properties.Settings.Default.Save();
            OnPropertyChanged(nameof(Types));

            OnPropertyChanged(nameof(TypesNames));
        }
        
        public ObservableCollection<String> Entries { get; set; } = new ObservableCollection<String>();

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
