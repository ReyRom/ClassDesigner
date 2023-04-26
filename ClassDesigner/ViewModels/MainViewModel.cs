using ClassDesigner.Helping;
using ClassDesigner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            PropertiesWindow window = new PropertiesWindow();
            window.Show();

            var files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Templates"));
            foreach (var file in files)
            {
                Templates.Add(new TemplateViewModel(file));
            }
        }

        public SettingsService SettingsService { get => SettingsService.Instance; }

        public ObservableCollection<TemplateViewModel> Templates { get; set; } = new ObservableCollection<TemplateViewModel>();

    }
}
