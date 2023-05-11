using ClassDesigner.Controls;
using ClassDesigner.Helping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class AssotiationDataViewModel:ViewModelBase, IConnectionData
    {
        public AssotiationDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            assotiationError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(assotiationError);

            Validate();
        }

        private ErrorViewModel assotiationError;

        public IErrorProvider ErrorSource { get; }
        public IEntry Source { get; }
        public IEntry Target { get; }


        private bool isValidSource = true;
        public bool IsValidSource
        {
            get
            {
                return isValidSource;
            }
            set
            {
                isValidSource = value;
                OnPropertyChanged(nameof(IsValidSource));
            }
        }


        public bool ValidateSource()
        {
            return IsValidSource = true;
        }

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();
            if (!ValidateSource())
            {
                sb.AppendLine("Тут не может быть ошибок");
            }
            assotiationError.Text = sb.ToString().TrimEnd();
        }

        public void ReleaseData()
        {
            if (assotiationError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(assotiationError);
                assotiationError = null;
            }
        }
    }
}
