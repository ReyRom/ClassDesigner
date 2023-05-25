using ClassDesigner.Helping;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace ClassDesigner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            PropertiesService.Instance.PropertyChanged += Instance_PropertyChanged;
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Templates\\Elements")))
            {
                var files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Templates\\Elements"));
                foreach (var file in files)
                {
                    Templates.Add(new TemplateViewModel(file));
                }
            }
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Templates\\Patterns")))
            {
                var files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Templates\\Patterns"));
                foreach (var file in files)
                {
                    PatternTemplates.Add(new TemplateViewModel(file));
                }
            }
        }

        public SettingsService SettingsService { get => SettingsService.Instance; }

        public PropertiesService PropertiesService { get => PropertiesService.Instance; }

        public ErrorService ErrorService { get => ErrorService.Instance; }

        public ObservableCollection<TemplateViewModel> Templates { get; set; } = new ObservableCollection<TemplateViewModel>();
        public ObservableCollection<TemplateViewModel> PatternTemplates { get; set; } = new ObservableCollection<TemplateViewModel>();

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



        Command openAboutCommand;
        public Command OpenAboutCommand => openAboutCommand ??= new Command(obj =>
        {
            MessageBox.MessageBox.Show("О программе", $"Class Designer - CASE-средство проектирования классов на основе UML-нотации. \n©Садовский Роман Викторович, 2023. v{Assembly.GetExecutingAssembly().GetName().Version}", MessageBox.MessageBoxButtons.Ok);
        });
    }
}
