using ClassDesigner.Helping;
using ClassDesigner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        public MainViewModel()
        {
            //PropertiesWindow window = new PropertiesWindow();
            //window.Show();
            PropertiesService.Instance.PropertyChanged += Instance_PropertyChanged;
            var files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Templates"));
            foreach (var file in files)
            {
                Templates.Add(new TemplateViewModel(file));
            }
        }

        public SettingsService SettingsService { get => SettingsService.Instance; }

        public PropertiesService PropertiesService { get => PropertiesService.Instance; }

        public ObservableCollection<TemplateViewModel> Templates { get; set; } = new ObservableCollection<TemplateViewModel>();

        private void Instance_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(PropertiesService));
        }
        public object Item { get => PropertiesService.Instance.Selected; }
        public double InterfaceScale
        {
            get
            {
                return Properties.Settings.Default.InterfaceScale;
            }
        }
    }
}
