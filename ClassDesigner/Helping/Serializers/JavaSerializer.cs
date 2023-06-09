﻿using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System.Linq;
using System.Text;

namespace ClassDesigner.Helping.Serializers
{
    public class JavaSerializer : ISerializer
    {
        public string Extension => "java";

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
                sb.Append("final");
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

            if (classView.Parents.OfType<ClassViewModel>().Count() > 0)
            {
                sb.Append(" extends ");

                sb.Append(string.Join(", ", classView.Parents.OfType<ClassViewModel>().Select(x => x.Name)));
            }
            if (classView.Parents.OfType<InterfaceViewModel>().Count() > 0)
            {
                sb.Append(" implements ");

                sb.Append(string.Join(", ", classView.Parents.OfType<InterfaceViewModel>().Select(x => x.Name)));
            }
            sb.Space();
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

            sb.Append("class");

            sb.Space();

            sb.Append(structView.Name);

            if (structView.Parents.OfType<InterfaceViewModel>().Count() > 0)
            {
                sb.Append(" implements ");
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

            if (interfaceView.Parents.OfType<InterfaceViewModel>().Count() > 0)
            {
                sb.Append(" extends ");
                sb.Append(string.Join(", ", interfaceView.Parents.OfType<InterfaceViewModel>().Select(x => x.Name)));
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

            sb.Append(string.IsNullOrWhiteSpace(attribute.Type) ? "int" : attribute.Type);
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
            if (property.IsGet)
            {
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

                sb.Append(string.IsNullOrWhiteSpace(property.Type) ? "int" : property.Type);
                sb.Space();

                sb.Append("Get" + property.Name+"()");

                sb.Space();

                sb.AppendLine();
                sb.Tab(tabLevel);
                sb.Append('{');
                sb.AppendLine();
                sb.Tab(tabLevel+1);
                if (!string.IsNullOrWhiteSpace(property.DefaultValue))
                {
                    sb.Append("return ");
                    sb.Append(property.DefaultValue);
                    sb.Append(";");
                }
                sb.AppendLine();
                sb.Tab(tabLevel);
                sb.Append('}');

                sb.AppendLine();
            }
           
            if (property.IsSet)
            {
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

                sb.Append("void");
                sb.Space();

                sb.Append("Set" + property.Name);
                sb.Append("("+(string.IsNullOrWhiteSpace(property.Type) ? "int" : property.Type)+" value)");

                sb.Space();

                sb.AppendLine();
                sb.Tab(tabLevel);
                sb.Append('{');
                sb.AppendLine();
                sb.Tab(tabLevel);
                sb.Append('}');
            }



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

        public string SerializeParameter(ParameterViewModel parameter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.IsNullOrWhiteSpace(parameter.Type) ? "int" : parameter.Type);

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
                VisibilityType.Internal => "",
                _ => ""
            };
        }
    }
}
