using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System.Linq;
using System.Text;

namespace ClassDesigner.Helping.Serializers
{
    public class JavaSerializer
    {
        public static string SerializeEntry(IEntry entry)
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

        public static string SerializeClass(ClassViewModel classView, int tabLevel = 0)
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

            if (classView.Parents.OfType<ClassViewModel>().Count() > 0)
            {
                sb.Space();
                sb.Append("extends");
                foreach (var parent in classView.Parents.OfType<ClassViewModel>())
                {
                    sb.Space();
                    sb.Append(parent.Name);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            if (classView.Parents.OfType<InterfaceViewModel>().Count() > 0)
            {
                sb.Space();
                sb.Append("implements");
                foreach (var parent in classView.Parents.OfType<InterfaceViewModel>())
                {
                    sb.Space();
                    sb.Append(parent.Name);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();

            foreach (var attr in classView.Attributes.OfType<FieldViewModel>())
            {
                sb.AppendLine(SerializeAttribute(attr, tabLevel + 1));
            }

            foreach (var attr in classView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
            }

            foreach (var attr in classView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
            }

            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public static string SerializeStruct(StructViewModel structView, int tabLevel = 0)
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
                sb.Space();
                sb.Append("implements");
                foreach (var parent in structView.Parents.OfType<InterfaceViewModel>())
                {
                    sb.Space();
                    sb.Append(parent.Name);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();

            foreach (var attr in structView.Attributes.OfType<FieldViewModel>())
            {
                sb.AppendLine(SerializeAttribute(attr, tabLevel + 1));
            }

            foreach (var attr in structView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
            }

            foreach (var attr in structView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
            }

            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public static string SerializeInterface(InterfaceViewModel interfaceView, int tabLevel = 0)
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
                sb.Space();
                sb.Append("extends");
                foreach (var parent in interfaceView.Parents.OfType<InterfaceViewModel>())
                {
                    sb.Space();
                    sb.Append(parent.Name);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');
            sb.AppendLine();
            foreach (var attr in interfaceView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
            }

            foreach (var attr in interfaceView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
            }

            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public static string SerializeEnum(EnumViewModel enumView, int tabLevel = 0)
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
                sb.AppendLine(enumChild.EnumString);
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public static string SerializeAttribute(FieldViewModel attribute, int tabLevel = 0)
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

        public static string SerializeProperty(PropertyViewModel property, int tabLevel = 0)
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


        public static string SerializeMethod(MethodViewModel method, int tabLevel = 0)
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

        public static string SerializeParameter(ParameterViewModel parameter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.IsNullOrWhiteSpace(parameter.Type) ? "int" : parameter.Type);

            sb.Space();

            sb.Append(parameter.Name);

            sb.Space();

            if (!string.IsNullOrWhiteSpace(parameter.DefaultValue))
            {
                sb.Append(" = ");
                sb.Append(parameter.DefaultValue);
            }

            return sb.ToString();
        }



        public static string ConvertVisibility(VisibilityType visibilityType)
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
