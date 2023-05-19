using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ErrorViewModel : ViewModelBase
    {
        private string text;
        private ErrorCriticalFor errorCriticalFor = ErrorCriticalFor.None;

        public IErrorProvider Source { get; set; }

        public ErrorCriticalFor ErrorCriticalFor
        {
            get => errorCriticalFor; set
            {
                errorCriticalFor = value;
                OnPropertyChanged(nameof(ErrorCriticalFor));
            }
        }
        public string Text
        {
            get => text; set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }
    public enum ErrorCriticalFor
    {
        None,
        CodeGeneration
    }
}
