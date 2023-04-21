using ClassDesigner.Helping;
using System.Collections.ObjectModel;

namespace ClassDesigner.ViewModels
{
    public class PropertiesCollectionViewModel : ViewModelBase
    {
        private object currentItem;

        public PropertiesCollectionViewModel()
        {
            PropertiesService.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Items));
        }

        public object Items { get => PropertiesService.Instance.SelectedCollection; }

        public object CurrentItem
        {
            get => currentItem; set
            {
                currentItem = value;
                OnPropertyChanged(nameof(CurrentItem));
            }
        }
    }
}
