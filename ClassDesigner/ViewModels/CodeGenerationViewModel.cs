using ClassDesigner.Helping;
using ClassDesigner.Models;
using System.Windows.Forms;

namespace ClassDesigner.ViewModels
{
    public class CodeGenerationViewModel : ViewModelBase
    {
        public LanguagesList LanguagesList { get; set; } = new LanguagesList();

        private Command generateCodeCommand;
        public Command GenerateCodeCommand => generateCodeCommand ??= new Command(obj =>
        {
            foreach (LanguagesList.Language item in LanguagesList)
            {
                if (item.IsSelected)
                {
                    var generator = new CodeGenerator(item.Name);
                    generator.GenerateCode(DataService.Instance.Entries, Folder);
                }
            }
        });

        private string folder;
        public string Folder
        {
            get => folder; set
            {
                folder = value;
                OnPropertyChanged(nameof(Folder));
            }
        }

        private Command selectFolderCommand;
        public Command SelectFolderCommand => selectFolderCommand ??= new Command(obj =>
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Folder = folderBrowserDialog.SelectedPath;
            }
        });

        public SettingsService SettingsService { get => SettingsService.Instance; }
    }
}
