using ClassDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
                    typesNames.Add(type.Name);
                }
                return typesNames;
            }
        }

        

        int classCounter = 0;
        int interfaceCounter = 0;
        int structCounter = 0;
        int enumCounter = 0;

        public void ProvideName(IEntry entry)
        {
            entry.Name = entry switch
            {
                ClassViewModel =>  "Class" + ++classCounter,
                InterfaceViewModel =>  "Interface" + ++interfaceCounter,
                StructViewModel =>  "Struct" + ++structCounter,
                EnumViewModel =>  "Enum" + ++enumCounter,
                _ => throw new InvalidOperationException()
            };
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

        public static bool CheckModifiedType(string type, string modified)
        {
            Regex regex = new Regex(@"^\w+<" + type + ">$");
            if (regex.IsMatch(modified))
            {
                return true;
            }
            regex = new Regex(@"^" + type + @"\[\]$");
            if (regex.IsMatch(modified))
            {
                return true;
            }
            regex = new Regex(@"^" + type + @"[*&]$");
            if (regex.IsMatch(modified))
            {
                return true;
            }
            return false;
        }

        private IEnumerable<IEntry> entries;

        public DataService()
        {
            errorViewModel = new ErrorViewModel();
            ErrorService.Instance.ObservableErrors.Add(errorViewModel);
        }

        ErrorViewModel errorViewModel;

        public void UpdateEntries(IEnumerable<IEntry> entries)
        {
            foreach (var entry in entries)
            {
                entry.PropertyChanged -= Entry_PropertyChanged;
            }

            this.entries = entries;

            foreach (var entry in entries)
            {
                entry.PropertyChanged += Entry_PropertyChanged;
            }

            var repeat = entries.GroupBy(x => x.Name).FirstOrDefault(g => g.Count() > 1);
            if (repeat is not null)
            {
                errorViewModel.Source = (IErrorProvider)repeat.First();
                errorViewModel.Text = $"Сущность с именем {repeat.Key} определена более одного раза";
            }

            OnPropertyChanged(nameof(Entries));
        }

        private void Entry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var repeat = entries.GroupBy(x => x.Name).FirstOrDefault(g => g.Count() > 1);
            if (repeat is not null)
            {
                errorViewModel.Source = (IErrorProvider)repeat.First();
                errorViewModel.Text = $"Сущность с именем {repeat.Key} определена более одного раза";
            }
        }

        public IEnumerable<IEntry> Entries { get => entries; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
