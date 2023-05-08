using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AppSettingsViewModel : ViewModelBase
    {
        

        public SettingsService SettingsService { get => SettingsService.Instance; }

        public DataService DataService { get => DataService.Instance; }

        private string newType;
        public string NewType
        {
            get => newType; set
            {
                newType = value;
                OnPropertyChanged(nameof(NewType));
            }
        }


        private Command removeTypeCommand;
        public Command RemoveTypeCommand => removeTypeCommand ??= new Command(obj =>
        {
            DataService.RemoveType((string)obj);
        });

        private Command addTypeCommand;
        public Command AddTypeCommand => addTypeCommand ??= new Command(obj =>
        {
            DataService.AddType((string)obj);
            NewType = string.Empty;
        }, obj => !String.IsNullOrWhiteSpace((string)obj));
    }
}
