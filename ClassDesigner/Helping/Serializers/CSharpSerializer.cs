﻿using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System.Linq;
using System.Text;

namespace ClassDesigner.Helping
{
    public class CSharpSerializer : ISerializer
    {
        public string Extension => "cs";
        public string SerializeEntry(IEntry entry)
        {
            switch (entry)
            {
                case ClassViewModel e:
                    return SerializeClass(e);
                case InterfaceViewModel e:
                    return SerializeInterface(e);
                case StructViewModel e:
                    return SerializeStruct(e);
                case EnumViewModel e:
                    return SerializeEnum(e);
                default:
                    return string.Empty;
            }
        }

        public string SerializeClass(ClassViewModel classView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(classView.Visibility));

            sb.Space();

            if (classView.IsStatic)
            {
                sb.Append("static");
                sb.Space();
            }
            if (classView.IsAbstract)
            {
                sb.Append("abstract");
                sb.Space();
            }

            sb.Append("class");

            sb.Space();

            sb.Append(classView.Name);

            if (classView.Parents.Count > 0)
            {
                sb.Append(" : ");

                sb.Append(string.Join(", ", classView.Parents.OfType<ClassViewModel>().Select(x => x.Name)));

                if (classView.Parents.Any(x => x is ClassViewModel) && classView.Parents.Any(x => x is InterfaceViewModel))
                {
                    sb.Append(", ");
                }
                sb.Append(string.Join(", ", classView.Parents.OfType<InterfaceViewModel>().Select(x=>x.Name)));
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();

            foreach (var attr in classView.Attributes.OfType<FieldViewModel>())
            {
                sb.AppendLine(SerializeAttribute(attr, tabLevel + 1));
                sb.AppendLine();
            }

            foreach (var attr in classView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
                sb.AppendLine();
            }


            foreach (var attr in classView.Actions.OfType<ConstructorViewModel>())
            {
                sb.AppendLine(SerializeConstructor(attr, tabLevel + 1));
                sb.AppendLine();
            }
            foreach (var attr in classView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
                sb.AppendLine();
            }

            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeStruct(StructViewModel structView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(structView.Visibility));

            sb.Space();

            sb.Append("struct");

            sb.Space();

            sb.Append(structView.Name);

            if (structView.Parents.Count > 0)
            {
                sb.Append(" : ");
                sb.Append(string.Join(", ", structView.Parents.Select(x => x.Name)));
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();

            foreach (var attr in structView.Attributes.OfType<FieldViewModel>())
            {
                sb.AppendLine(SerializeAttribute(attr, tabLevel + 1));
                sb.AppendLine();
            }

            foreach (var attr in structView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
                sb.AppendLine();
            }
            foreach (var attr in structView.Actions.OfType<ConstructorViewModel>())
            {
                sb.AppendLine(SerializeConstructor(attr, tabLevel + 1));
                sb.AppendLine();
            }

            foreach (var attr in structView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
                sb.AppendLine();
            }

            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeInterface(InterfaceViewModel interfaceView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(interfaceView.Visibility));

            sb.Space();

            sb.Append("interface");

            sb.Space();

            sb.Append(interfaceView.Name);

            if (interfaceView.Parents.Count > 0)
            {
                sb.Append(" : ");
                sb.Append(string.Join(", ", interfaceView.Parents.Select(x => x.Name)));
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();
            foreach (var attr in interfaceView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
                sb.AppendLine();
            }

            foreach (var attr in interfaceView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
                sb.AppendLine();
            }

            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeEnum(EnumViewModel enumView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(enumView.Visibility));

            sb.Space();

            sb.Append("enum");

            sb.Space();

            sb.Append(enumView.Name);

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();
            foreach (var enumChild in enumView.EnumChildren)
            {
                sb.Tab(tabLevel + 1);
                sb.Append(enumChild.Name);
                if (!string.IsNullOrWhiteSpace(enumChild.Value))
                {
                    sb.Append(" = " + enumChild.Value);
                }
                if (enumChild != enumView.EnumChildren.Last())
                {
                    sb.Append(',');
                }
                sb.AppendLine();
            }
            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeAttribute(FieldViewModel attribute, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(attribute.Visibility));

            sb.Space();

            if (attribute.IsStatic)
            {
                sb.Append("static");
                sb.Space();
            }

            sb.Append(string.IsNullOrWhiteSpace(attribute.Type) ? "object" : attribute.Type);
            sb.Space();

            sb.Append(attribute.Name);

            if (!string.IsNullOrWhiteSpace(attribute.DefaultValue))
            {
                sb.Append(" = ");
                sb.Append(attribute.DefaultValue);
            }

            sb.Append(";");
            return sb.ToString();
        }

        public string SerializeProperty(PropertyViewModel property, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(property.Visibility));

            sb.Space();

            if (property.IsAbstract)
            {
                sb.Append("abstract");
                sb.Space();
            }

            if (property.IsStatic)
            {
                sb.Append("static");
                sb.Space();
            }

            sb.Append(string.IsNullOrWhiteSpace(property.Type) ? "object" : property.Type);
            sb.Space();

            sb.Append(property.Name);

            sb.Space();

            sb.Append($"{{ {(property.IsGet ? "get; " : "")}{(property.IsSet ? "set; " : "")}}}");

            if (!string.IsNullOrWhiteSpace(property.DefaultValue))
            {
                sb.Append(" = ");
                sb.Append(property.DefaultValue);
                sb.Append(";");
            }
            return sb.ToString();
        }


        public string SerializeConstructor(ConstructorViewModel method, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(method.Visibility));

            sb.Space();

            sb.Append(method.Name);

            sb.Append($"({string.Join(", ", method.Parameters.Select(x => SerializeParameter(x)))})");

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeMethod(MethodViewModel method, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(method.Visibility));

            sb.Space();

            if (method.IsAbstract)
            {
                sb.Append("abstract");
                sb.Space();
            }

            if (method.IsStatic)
            {
                sb.Append("static");
                sb.Space();
            }

            sb.Append(string.IsNullOrWhiteSpace(method.Type) ? "void" : method.Type);
            sb.Space();

            sb.Append(method.Name);

            sb.Append($"({string.Join(", ", method.Parameters.Select(x => SerializeParameter(x)))})");

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeParameter(ParameterViewModel parameter)
        {
            StringBuilder sb = new StringBuilder();
            if (parameter.IsOut)
            {
                sb.Append("out");

                sb.Space();
            }

            sb.Append(string.IsNullOrWhiteSpace(parameter.Type) ? "object" : parameter.Type);

            sb.Space();

            sb.Append(parameter.Name);

            

            if (!string.IsNullOrWhiteSpace(parameter.DefaultValue))
            {
                sb.Append(" = ");
                sb.Append(parameter.DefaultValue);
            }

            return sb.ToString();
        }



        public string ConvertVisibility(VisibilityType visibilityType)
        {
            return visibilityType switch
            {
                VisibilityType.Public => "public",
                VisibilityType.Private => "private",
                VisibilityType.Protected => "protected",
                VisibilityType.Internal => "internal",
                _ => ""
            };
        }
    }
}
