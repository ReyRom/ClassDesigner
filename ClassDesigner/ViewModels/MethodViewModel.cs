using ClassDesigner.Helping;
using ClassDesigner.Models;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ClassDesigner.ViewModels
{
    public class MethodViewModel : ViewModelBase
    {


        public MethodViewModel()
        {
            Name = "Method";
            Visibility = VisibilityType.Private;
            Parameters = new ObservableCollection<ParameterViewModel>();
        }
        public bool IsStatic { get; set; } = false;
        public string Name { get; set; } = "Method";
        public VisibilityType Visibility { get; set; } = VisibilityType.Private;
        public string Type { get; set; }
        public ObservableCollection<ParameterViewModel> Parameters { get; set; }

        public string MethodString
        {
            get => this.ToString();
            set
            {
                ParseFromString(value);
            }
        }

        public void ParseFromString(string value)
        {
            var m = MatchMethodString(value);
            this.Name = m.Groups["Name"].Value;
            this.Type = m.Groups["Type"].Value;
            this.Visibility = (VisibilityType)(m.Groups["Visible"].Value[0]);
            this.Parameters = ParseParameters(m.Groups["Parameters"].Value);
        }

        private ObservableCollection<ParameterViewModel> ParseParameters(string value)
        {
            ObservableCollection<ParameterViewModel> parameters = new ObservableCollection<ParameterViewModel>();
            foreach (var item in value.Split(", "))
            {
                parameters.Add(ParameterViewModel.ParseFromString(item));
            }
            return parameters;
        }

        public static Match MatchMethodString(string value)
        {
            return Regex.Match(value, @"^(?<Visible>[-+#~])\s(?<Name>\w+)([(](?<Parameters>(\w+(\s:\s\w+){0,1}(,\s\w+(\s:\s\w+))*){0,1})[)])(\s:\s(?<Type>\w+)){0,1}$");
        }

        public override string ToString()
        {
            return (char)Visibility + " " + Name + "(" + String.Join(", ", Parameters) + ")" + (string.IsNullOrWhiteSpace(Type) ? "" : " : " + Type);
        }

        private Command addParameterCommand;
        public Command AddParameterCommand => addParameterCommand ??= new Command(obj =>
        {
            this.Parameters.Add(new ParameterViewModel());
        });

        private Command removeParameterCommand;
        public Command RemoveParameterCommand => removeParameterCommand ??= new Command(obj =>
        {
            this.Parameters.Remove((ParameterViewModel)obj);
        });
    }
}
