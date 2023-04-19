using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.ViewModels
{
    public class ClassViewModel:ViewModelBase
    {
        public string Header { get; set; } = "Class";
        public VisibilityType Visibility { get; set; } = VisibilityType.Public;
        public bool IsStatic { get; set; } = false;
        public ObservableCollection<AttributeViewModel> Attributes { get; set; } = new ObservableCollection<AttributeViewModel>() { new AttributeViewModel() };
        public bool IsAbstract { get; set; } = false;
        public ObservableCollection<Stereotype> Stereotypes { get; set; } = new ObservableCollection<Stereotype>();
        public ObservableCollection<MethodViewModel> Methods { get; set; } = new ObservableCollection<MethodViewModel>() { new MethodViewModel() };



        Command addAttributeCommand;
        public Command AddAttributeCommand
        {
            get => addAttributeCommand ?? (addAttributeCommand = new Command(obj =>
            {
                this.Attributes.Add(new AttributeViewModel());
            }));
        }

        Command addMethodCommand;
        public Command AddMethodCommand
        {
            get => addMethodCommand ?? (addMethodCommand = new Command(obj =>
            {
                this.Methods.Add(new MethodViewModel());
            }));
        }

        Command addStereotypeCommand;
        public Command AddStereotypeCommand
        {
            get => addStereotypeCommand ?? (addStereotypeCommand = new Command(obj =>
            {
                if (this.Stereotypes.Contains((Stereotype)obj))
                {
                    this.Stereotypes.Remove((Stereotype)obj);
                }
                else
                {
                    this.Stereotypes.Add((Stereotype)obj);
                }
            }));
        }
    }
}
