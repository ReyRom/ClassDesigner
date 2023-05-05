using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ClassDesigner.Helping
{
    public class SettingsService : INotifyPropertyChanged
    {
        private static SettingsService instance;
        private RelationType relationType = RelationType.Association;

        ResourceDictionary LightTheme;
        ResourceDictionary DarkTheme;
        

        public bool IsDarkTheme 
        {
            get
            {
                return Properties.Settings.Default.IsDarkTheme;
            }
            set
            {
                Properties.Settings.Default.IsDarkTheme = value;
                Properties.Settings.Default.Save();
                UpdateTheme();
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }

        public SettingsService()
        {
            var lightUri = new Uri("Resources/Themes/LightTheme.xaml", UriKind.Relative);
            var darkUri = new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative);

            LightTheme = Application.LoadComponent(lightUri) as ResourceDictionary;
            DarkTheme = Application.LoadComponent(darkUri) as ResourceDictionary;

            UpdateTheme();
        }

        private void UpdateTheme()
        {
            if (IsDarkTheme) 
            {
                Application.Current.Resources.MergedDictionaries.Remove(LightTheme);
                Application.Current.Resources.MergedDictionaries.Add(DarkTheme);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Remove(DarkTheme);
                Application.Current.Resources.MergedDictionaries.Add(LightTheme);
            }
        }

        public static SettingsService Instance { get => instance ??= new SettingsService(); }
        public RelationType RelationType
        {
            get => relationType; set
            {
                relationType = value;
                OnPropertyChanged("RelationType");
            }
        }

        public double InterfaceScale
        {
            get
            {
                return Properties.Settings.Default.InterfaceScale;
            }
            set
            {
                Properties.Settings.Default.InterfaceScale = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(InterfaceScale));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
