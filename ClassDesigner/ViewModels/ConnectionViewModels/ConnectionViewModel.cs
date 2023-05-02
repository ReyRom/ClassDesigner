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
                    ConnectionData = new AssotiationDataViewModel(sourceEntry, targetEntry);
                    break;
                case RelationType.Aggregation:
                    ConnectionData = new AggregationDataViewModel(sourceEntry, targetEntry);
                    break;
                case RelationType.Composition:
                    ConnectionData = new CompositionDataViewModel(sourceEntry, targetEntry);
                    break;
                case RelationType.Generalization:
                    ConnectionData = new GeneralizationDataViewModel(sourceEntry, targetEntry);
                    break;
                case RelationType.Realization:
                    ConnectionData = new RealizationDataViewModel(this);
                    break;
                case RelationType.Dependency:
                    ConnectionData = new DependencyDataViewModel(sourceEntry, targetEntry);
                    break;
                default:
                    break;
            }
        }

        public IEntry SourceEntry { get; set; }
        public IEntry TargetEntry { get; set; }
        public RelationType RelationType { get; set; }

        public IConnectionData ConnectionData { get; set; }
    }
}
