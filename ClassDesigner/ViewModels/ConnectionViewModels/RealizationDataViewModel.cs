using ClassDesigner.Helping;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassDesigner.ViewModels
{
    public class RealizationDataViewModel : ViewModelBase, IConnectionData
    {
        public RealizationDataViewModel(ConnectionViewModel connection)
        {
            Source = connection.SourceEntry;
            Target = connection.TargetEntry;
            ErrorSource = connection;

            realizationError = new ErrorViewModel() { Source = ErrorSource };
            ErrorService.Instance.ObservableErrors.Add(realizationError);
            if (ValidateSource()) 
            {
                ((IHaveAttributes)Target).Attributes.CollectionChanged += SourceTarget_CollectionChanged;
                ((IHaveActions)Target).Actions.CollectionChanged += SourceTarget_CollectionChanged;
                ((IHaveAttributes)Source).Attributes.CollectionChanged += SourceTarget_CollectionChanged;
                ((IHaveActions)Source).Actions.CollectionChanged += SourceTarget_CollectionChanged;

                ((IInheritor)Source).Parents.Add((IInheritable)Target);
            }
            
            Validate();
        }

        public void ReleaseData()
        {
            if (realizationError != null)
            {
                ErrorService.Instance.ObservableErrors.Remove(realizationError);
                ((IInheritor)Source).Parents.Remove((IInheritable)Target);
                realizationError = null;
            }
        }

        private void SourceTarget_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Validate();
        }

        public List<IAttribute> Attributes => ((IHaveAttributes)Target).Attributes.Where(x => x.Visibility != Models.VisibilityType.Private).ToList();
        public List<IAction> Actions => ((IHaveActions)Target).Actions.Where(x => x.Visibility != Models.VisibilityType.Private).ToList();

        public IEntry Source { get; }
        public IEntry Target { get; }

        public IErrorProvider ErrorSource { get; }

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
            return IsValidSource = Target is InterfaceViewModel && Source is ClassViewModel;
        }

        private ErrorViewModel realizationError;

        public void Validate()
        {
            StringBuilder sb = new StringBuilder();
            if (!ValidateSource())
            {
                sb.AppendLine("Источником реализации может быть только класс, а целью только интерфейс");
            }
            else
            {
                if (!ValidateAttributes())
                {
                    sb.AppendLine("Реализованы не все атрибуты");
                }
                if (!ValidateActions())
                {
                    sb.AppendLine("Реализованы не все действия");
                }
            }
            realizationError.Text = sb.ToString().TrimEnd();
        }

        private bool ValidateActions()
        {
            var isValid = true;
            foreach (var item in Actions)
            {
                if (!isValid) break;
                isValid = (Source as IHaveActions).Actions.Contains(item);
            }
            return isValid;
        }

        private bool ValidateAttributes()
        {
            var isValid = true;
            foreach (var item in Attributes)
            {
                if (!isValid) break;
                isValid = (Source as IHaveAttributes).Attributes.Contains(item);
            }
            return isValid;
        }

        Command realizeAttributeCommand;
        public Command RealizeAttributeCommand => realizeAttributeCommand ??= new Command(obj =>
        {
            IAttribute realize = obj as IAttribute;
            if (obj is FieldViewModel f)
            {
                realize = new FieldViewModel(Source, f);
            }
            else if (obj is PropertyViewModel p)
            {
                realize = new PropertyViewModel(Source, p);
            }
            (Source as IHaveAttributes).Attributes.Add(realize);
        }, obj => !(Source as IHaveAttributes).Attributes.Contains(obj as IAttribute));
        Command realizeMethodCommand;
        public Command RealizeMethodCommand => realizeMethodCommand ??= new Command(obj =>
        {
            IAction realize = obj as IAction;
            if (obj is MethodViewModel m)
            {
                realize = new MethodViewModel(Source, m);
            }
            else if (obj is ConstructorViewModel c)
            {
                realize = new ConstructorViewModel(Source, c);
            }
            (Source as IHaveActions).Actions.Add(realize);
        }, obj => !(Source as IHaveActions).Actions.Contains(obj as IAction));
    }
}
