using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;

namespace ClassDesigner.Helping.Serializers
{
    internal class PythonSerializer : ISerializer
    {
        public string Extension => "py";

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

            //sb.Append(ConvertVisibility(classView.Visibility));

            //sb.Space();

            //if (classView.IsAbstract)
            //{
            //    sb.Append("from abc import ABCMeta, abstractmethod, abstractproperty");
            //    sb.AppendLine();
            //}
            sb.Tab(tabLevel);

            sb.Append("class");

            sb.Space();

            sb.Append(classView.Name);

            if (classView.Parents.Count > 0)
            {
                sb.Space();
                sb.Append("(");
                sb.Append(string.Join(", ", classView.Parents.OfType<ClassViewModel>().Select(x => x.Name)));
                if (classView.Parents.Any(x => x is ClassViewModel) && classView.Parents.Any(x => x is InterfaceViewModel))
                {
                    sb.Append(", ");
                }
                sb.Append(string.Join(", ", classView.Parents.OfType<InterfaceViewModel>().Select(x => x.Name)));
                sb.Append(")");
            }
            sb.Append(":");
            sb.AppendLine();

            //if (classView.IsAbstract)
            //{
            //    sb.Tab(tabLevel);
            //    sb.Append("__metaclass__ = ABCMeta");
            //    sb.AppendLine();
            //}

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

            return sb.ToString();
        }

        public string SerializeStruct(StructViewModel structView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            //sb.Append(ConvertVisibility(classView.Visibility));

            //sb.Space();

            //if (classView.IsAbstract)
            //{
            //    sb.Append("from abc import ABCMeta, abstractmethod, abstractproperty");
            //    sb.AppendLine();
            //}
            sb.Tab(tabLevel);

            sb.Append("class");

            sb.Space();

            sb.Append(structView.Name);

            if (structView.Parents.Count > 0)
            {
                sb.Append("(");
                sb.Append(string.Join(", ", structView.Parents.OfType<ClassViewModel>().Select(x => x.Name)));
                if (structView.Parents.Any(x => x is ClassViewModel) && structView.Parents.Any(x => x is InterfaceViewModel))
                {
                    sb.Append(", ");
                }
                sb.Append(string.Join(", ", structView.Parents.OfType<InterfaceViewModel>().Select(x => x.Name)));
                sb.Append(")");
            }
            sb.Append(":");
            sb.AppendLine();

            //if (classView.IsAbstract)
            //{
            //    sb.Tab(tabLevel);
            //    sb.Append("__metaclass__ = ABCMeta");
            //    sb.AppendLine();
            //}

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

            return sb.ToString();
        }

        public string SerializeInterface(InterfaceViewModel interfaceView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            //sb.Append(ConvertVisibility(classView.Visibility));

            //sb.Space();

            //if (classView.IsAbstract)
            //{
            //    sb.Append("from abc import ABCMeta, abstractmethod, abstractproperty");
            //    sb.AppendLine();
            //}
            sb.Tab(tabLevel);

            sb.Append("class");

            sb.Space();

            sb.Append(interfaceView.Name);


            if (interfaceView.Parents.Count > 0)
            {
                sb.Space();
                sb.Append("(");
                sb.Append(string.Join(", ", interfaceView.Parents.OfType<ClassViewModel>().Select(x => x.Name)));
                if (interfaceView.Parents.Any(x => x is ClassViewModel) && interfaceView.Parents.Any(x => x is InterfaceViewModel))
                {
                    sb.Append(", ");
                }
                sb.Append(string.Join(", ", interfaceView.Parents.OfType<InterfaceViewModel>().Select(x => x.Name)));
                sb.Append(")");
            }
            sb.Append(":");
            sb.AppendLine();

            //if (classView.IsAbstract)
            //{
            //    sb.Tab(tabLevel);
            //    sb.Append("__metaclass__ = ABCMeta");
            //    sb.AppendLine();
            //}

            foreach (var attr in interfaceView.Attributes.OfType<FieldViewModel>())
            {
                sb.AppendLine(SerializeAttribute(attr, tabLevel + 1));
                sb.AppendLine();
            }

            foreach (var attr in interfaceView.Attributes.OfType<PropertyViewModel>())
            {
                sb.AppendLine(SerializeProperty(attr, tabLevel + 1));
                sb.AppendLine();
            }
            foreach (var attr in interfaceView.Actions.OfType<ConstructorViewModel>())
            {
                sb.AppendLine(SerializeConstructor(attr, tabLevel + 1));
                sb.AppendLine();
            }
            foreach (var attr in interfaceView.Actions.OfType<MethodViewModel>())
            {
                sb.AppendLine(SerializeMethod(attr, tabLevel + 1));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string SerializeEnum(EnumViewModel enumView, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.AppendLine("from enum import Enum");

            //sb.Append(ConvertVisibility(enumView.Visibility));

            sb.Tab(tabLevel);

            sb.Append("class");

            sb.Space();

            sb.Append(enumView.Name);
            sb.Append("(Enum):");
            sb.AppendLine();
            foreach (var enumChild in enumView.EnumChildren)
            {
                sb.Tab(tabLevel + 1);
                sb.Append(enumChild.Name);
                if (!string.IsNullOrWhiteSpace(enumChild.Value))
                {
                    sb.Append(" = " + enumChild.Value);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string SerializeAttribute(FieldViewModel attribute, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append(ConvertVisibility(attribute.Visibility));

            sb.Append(attribute.Name);
            sb.Append(" = ");

            if (!string.IsNullOrWhiteSpace(attribute.DefaultValue))
            {
                sb.Append(attribute.DefaultValue);
            }
            else
            {
                sb.Append("0");
            }
            return sb.ToString();
        }

        public string SerializeProperty(PropertyViewModel property, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            if (property.IsGet && property.IsSet)
            {
                sb.Tab(tabLevel);

                //if (property.IsAbstract)
                //{
                //    sb.Append("abstract");
                //    sb.Space();
                //}

                //sb.Space();

                //if (property.IsStatic)
                //{
                //    sb.Append("static");
                //    sb.Space();
                //}
                sb.AppendLine("@property");
                sb.Tab(tabLevel);
                sb.Append("def");
                sb.Space();
                sb.Append(ConvertVisibility(property.Visibility));
                sb.Append(property.Name);
                sb.AppendLine("(self):");
                if (!string.IsNullOrWhiteSpace(property.DefaultValue))
                {
                    sb.Tab(tabLevel + 1);
                    sb.Append("return ");
                    sb.Append(property.DefaultValue);
                }
                else
                {
                    sb.Tab(tabLevel + 1);
                    sb.Append("pass");
                }
                sb.AppendLine();
                sb.Tab(tabLevel);
                sb.Append($"@{property.Name}.setter");
                sb.AppendLine();
                sb.Tab(tabLevel);
                sb.Append("def");
                sb.Space();
                sb.Append(ConvertVisibility(property.Visibility));
                sb.Append(property.Name);
                sb.AppendLine("(self, value):");
                sb.Tab(tabLevel + 1);
                sb.Append("pass");
            }

            else if (property.IsGet)
            {
                sb.Tab(tabLevel);

                //if (property.IsAbstract)
                //{
                //    sb.Append("abstract");
                //    sb.Space();
                //}

                //sb.Space();

                //if (property.IsStatic)
                //{
                //    sb.Append("static");
                //    sb.Space();
                //}
                sb.AppendLine("@property");
                sb.Tab(tabLevel);
                sb.Append("def");
                sb.Space();
                sb.Append(ConvertVisibility(property.Visibility));
                sb.Append(property.Name);
                sb.AppendLine("(self):");
                if (!string.IsNullOrWhiteSpace(property.DefaultValue))
                {
                    sb.Tab(tabLevel + 1);
                    sb.Append("return ");
                    sb.Append(property.DefaultValue);
                }
                else
                {
                    sb.Tab(tabLevel + 1);
                    sb.Append("pass");
                }

            }
            else if (property.IsSet)
            {
                //if (property.IsAbstract)
                //{
                //    sb.Append("abstract");
                //    sb.Space();
                //}

                //sb.Space();

                //if (property.IsStatic)
                //{
                //    sb.Append("static");
                //    sb.Space();
                //}
                sb.Tab(tabLevel);
                sb.Append("def");
                sb.Space();
                sb.Append(ConvertVisibility(property.Visibility));
                sb.Append("set_" + property.Name);
                sb.AppendLine("(self, value):");
                sb.Tab(tabLevel + 1);
                sb.Append("pass");
            }


            return sb.ToString();
        }


        public string SerializeMethod(MethodViewModel method, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append("def");
            sb.Space();

            sb.Append(ConvertVisibility(method.Visibility));

            //sb.Space();

            //if (method.IsAbstract)
            //{
            //    sb.Append("abstract");
            //    sb.Space();
            //}

            //if (method.IsStatic)
            //{
            //    sb.Append("static");
            //    sb.Space();
            //}


            sb.Append(method.Name);

            sb.Append("(self");
            if (method.Parameters.Count > 0)
            {
                sb.Append(", ");
                sb.Append(string.Join(", ", method.Parameters.Select(x => SerializeParameter(x))));
            }
            sb.Append("):");
            sb.AppendLine();
            sb.Tab(tabLevel + 1);
            sb.Append("pass");
            //if (!string.IsNullOrWhiteSpace(method.Type))
            //{
            //    sb.Append(':');
            //    sb.Space();
            //    sb.Append(method.Type);
            //}

            //sb.Space();
            //sb.Append('{');

            //sb.AppendLine();
            //sb.Tab(tabLevel);
            //sb.Append('}');

            return sb.ToString();
        }

        public string SerializeConstructor(ConstructorViewModel method, int tabLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Tab(tabLevel);

            sb.Append("def");
            sb.Space();

            sb.Append("__init__");

            sb.Append("(self");
            if (method.Parameters.Count > 0)
            {
                sb.Append(", ");
                sb.Append(string.Join(", ", method.Parameters.Select(x => SerializeParameter(x))));
            }
            sb.Append("):");
            sb.AppendLine();
            sb.Tab(tabLevel + 1);
            sb.Append("pass");
            //if (!string.IsNullOrWhiteSpace(method.Type))
            //{
            //    sb.Append(':');
            //    sb.Space();
            //    sb.Append(method.Type);
            //}

            //sb.Space();
            //sb.Append('{');

            //sb.AppendLine();
            //sb.Tab(tabLevel);
            //sb.Append('}');

            return sb.ToString();
        }

        public string SerializeParameter(ParameterViewModel parameter)
        {
            StringBuilder sb = new StringBuilder();
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
                VisibilityType.Public => "",
                VisibilityType.Private => "_",
                VisibilityType.Protected => "__",
                VisibilityType.Internal => "",
                _ => ""
            };
        }
    }
}
