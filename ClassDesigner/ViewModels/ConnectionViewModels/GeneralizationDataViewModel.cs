using ClassDesigner.Controls;
using ClassDesigner.Helping;
using ClassDesigner.Helping.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class GeneralizationDataViewModel:ViewModelBase, IConnectionData
    {
        public GeneralizationDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            generalizationError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(generalizationError);

            Validate();
        }

        public List<IAttribute> Fields => ((IHaveAttributes)Target).Attributes.Where(x => x.Visibility != Models.VisibilityType.Private).ToList();
        public List<IAction> Methods => ((IHaveActions)Target).Actions.Where(x => x.Visibility != Models.VisibilityType.Private).ToList();

        private ErrorViewModel generalizationError;
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
            return IsValidSource = Target is InterfaceViewModel && Source is InterfaceViewModel
                          || Target is ClassViewModel && Source is ClassViewModel;
        }

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();
            if (!ValidateSource())
            {
                sb.AppendLine("Наследоваться могут только классы и интерфейсы");
            }
            generalizationError.Text = sb.ToString().TrimEnd();
        }

        public void ReleaseData()
        {
            if (generalizationError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(generalizationError);
                generalizationError = null;
            }
        }
    }
}
