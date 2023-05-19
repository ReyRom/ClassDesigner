using ClassDesigner.Helping;
using ClassDesigner.Models;
using System.Linq;
using System.Windows.Forms;

namespace ClassDesigner.ViewModels
{
    public class CodeGenerationViewModel : ViewModelBase
    {
        public LanguagesList LanguagesList { get; set; } = new LanguagesList();

        private Command generateCodeCommand;
        public Command GenerateCodeCommand => generateCodeCommand ??= new Command(obj =>
        {
            try
            {
                if (!LanguagesList.IsAnySelected)
                {
                    throw new System.Exception("Не выбран ни один язык для генерации");
                }
                if (ErrorService.Instance.Errors.Any(x => x.ErrorCriticalFor == ErrorCriticalFor.CodeGeneration))
                {
                    throw new System.Exception("В проекте присутствуют ошибки критичные для генерации кода");
                }
                foreach (LanguagesList.Language item in LanguagesList)
                {
                    if (item.IsSelected)
                    {
                        var generator = new CodeGenerator(item.Name);
                        generator.GenerateCode(DataService.Instance.Entries, Folder);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.MessageBox.Show("Ошибка", ex.Message, MessageBox.MessageBoxButtons.Ok);
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
