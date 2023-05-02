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

        public IErrorProvider Source { get; set; }

        public string Text
        {
            get => text; set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }
}
