using ClassDesigner.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ClassDesigner.Helping.Services
{
    public class ErrorService : INotifyPropertyChanged
    {
        private static ErrorService instance;

        public ErrorService()
        {
            ObservableErrors.CollectionChanged += Errors_CollectionChanged;
        }

        private void Errors_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems is not null)
            {
                foreach (ErrorViewModel error in e.OldItems)
                {
                    error.PropertyChanged -= Error_PropertyChanged;
                }
            }
            if (e.NewItems is not null)
            {
                foreach (ErrorViewModel error in e.NewItems)
                {
                    error.PropertyChanged += Error_PropertyChanged;
                }
            }
            OnPropertyChanged(nameof(Errors));
            OnPropertyChanged(nameof(ErrorsCount));
        }

        private void Error_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Errors));
            OnPropertyChanged(nameof(ErrorsCount));
        }

        public static ErrorService Instance { get => instance ??= new ErrorService(); }

        public ObservableCollection<ErrorViewModel> ObservableErrors { get; set; } = new ObservableCollection<ErrorViewModel>();

        public List<ErrorViewModel> Errors { get => ObservableErrors.Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList(); }

        public int ErrorsCount { get => Errors.Count(); }



        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
