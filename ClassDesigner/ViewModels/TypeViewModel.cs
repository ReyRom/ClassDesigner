using ClassDesigner.Helping;
using System.Collections.ObjectModel;

namespace ClassDesigner.ViewModels
{
    public class TypeViewModel : ViewModelBase
    {
        private string type = string.Empty;

        public string Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string NewType
        {
            get => type;
            set
            {
                if (Type != null)
                {
                    return;
                }
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!Values.Contains(value))
                    {
                        Values.Add(value);
                    }
                }
                Type = value;
            }
        }


        public ObservableCollection<string> Values => DataService.Instance.TypesNames;
    }
}
