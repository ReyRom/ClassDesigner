using ClassDesigner.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ClassDesigner.Helping
{
    public class SelectionService:INotifyPropertyChanged
    {
        DesignerCanvas designerCanvas;
        private List<ISelectable> selection = null!;
        public List<ISelectable> Selection { get => selection ??= new List<ISelectable>(); }

        public SelectionService(DesignerCanvas designerCanvas)
        {
            this.designerCanvas = designerCanvas;
            PropertyChanged += (s, e) => { SetProperties(); };
        }

        public void SetProperties()
        {
            if (Selection.LastOrDefault() is DesignerItem item)
            {
                PropertiesService.Instance.Selected = item.Content;
            }

            else if (Selection.LastOrDefault() is Connection connection)
            {
                PropertiesService.Instance.Selected = connection.ConnectionViewModel;
            }

            else
            {
                PropertiesService.Instance.Selected = null;
            }
        }

        public void SelectAll()
        {
            ClearSelection();
            Selection.AddRange(designerCanvas.Children.OfType<ISelectable>());
            Selection.ForEach(item => item.IsSelected = true);
            OnPropertyChanged(nameof(Selection));
        }

        public void SelectItem(ISelectable item)
        {
            ClearSelection();
            Selection.Add(item);
            item.IsSelected = true;
            OnPropertyChanged(nameof(Selection));
        }

        public void AddSelection(ISelectable item)
        {
            Selection.Add(item);
            item.IsSelected = true;
            OnPropertyChanged(nameof(Selection));
        }

        public void RemoveSelection(ISelectable item)
        {
            Selection.Remove(item);
            item.IsSelected = false;
            OnPropertyChanged(nameof(Selection));
        }

        public void ClearSelection()
        {
            Selection.ForEach(x => x.IsSelected = false);
            Selection.Clear();
            OnPropertyChanged(nameof(Selection));
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
