using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Linq;
using System.Threading;
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
                if (DataService.Instance.Entries is null)
                {
                    throw new Exception("В рабочей области нет сущностей для генерации кода");
                }
                if (string.IsNullOrWhiteSpace(Folder))
                {
                    throw new Exception("Указан некорректный путь");
                }
                if (!LanguagesList.IsAnySelected)
                {
                    throw new Exception("Не выбран ни один язык для генерации");
                }
                foreach (LanguagesList.Language item in LanguagesList)
                {
                    if (item.IsSelected)
                    {
                        var generator = new CodeGenerator(item.Name);
                        item.IsSuccess = generator.GenerateCode(DataService.Instance.Entries, Folder);
                    }
                }
                MessageBox.MessageBox.Show("Успех", "Операция выполнена", MessageBox.MessageBoxButtons.Ok, App.CodeGenerationWindow);
            }
            catch (Exception ex)
            {
                MessageBox.MessageBox.Show("Ошибка", ex.Message, MessageBox.MessageBoxButtons.Ok, App.CodeGenerationWindow);
            }
        });

        private Command resetCommand;
        public Command ResetCommand => resetCommand ??= new Command(obj =>
        {
            foreach (LanguagesList.Language item in LanguagesList)
            {
                item.IsSelected = false;
                item.IsSuccess = null;
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

        public ErrorService ErrorService { get => ErrorService.Instance; }
    }
}
