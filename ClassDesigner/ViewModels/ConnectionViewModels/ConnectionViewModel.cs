using ClassDesigner.Helping;
using ClassDesigner.Models;

namespace ClassDesigner.ViewModels
{
    public class ConnectionViewModel : ViewModelBase, IErrorProvider
    {
        public ConnectionViewModel(IEntry sourceEntry, IEntry targetEntry, RelationType relationType)
        {
            SourceEntry = sourceEntry;
            TargetEntry = targetEntry;
            RelationType = relationType;
            switch (RelationType)
            {
                case RelationType.Association:
                    ConnectionData = new AssotiationDataViewModel(this);
                    break;
                case RelationType.Aggregation:
                    ConnectionData = new AggregationDataViewModel(this);
                    break;
                case RelationType.Composition:
                    ConnectionData = new CompositionDataViewModel(this);
                    break;
                case RelationType.Generalization:
                    ConnectionData = new GeneralizationDataViewModel(this);
                    break;
                case RelationType.Realization:
                    ConnectionData = new RealizationDataViewModel(this);
                    break;
                case RelationType.Dependency:
                    ConnectionData = new DependencyDataViewModel(this);
                    break;
                default:
                    break;
            }
        }

        public IEntry SourceEntry { get; set; }
        public IEntry TargetEntry { get; set; }
        public RelationType RelationType { get; set; }

        public IConnectionData ConnectionData { get; set; }

        public void Release()
        {
            ConnectionData.ReleaseData();
        }
    }
}
