using ClassDesigner.Models;
using ClassDesigner.ViewModels;
using System.Linq;
using System.Text;

namespace ClassDesigner.Helping
{
    class CSharpSerializer
    {
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

            sb.Append(classView.Name);

            //наследование

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('{');

            foreach (var attr in classView.Attributes.OfType<AttributeViewModel>())
            {
                sb.AppendLine(SerializeAttribute(attr, tabLevel + 1));
            }

            sb.AppendLine();
            sb.Tab(tabLevel);
            sb.Append('}');

            return sb.ToString();
        }

        public string SerializeAttribute(AttributeViewModel attribute, int tabLevel = 0)
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

            sb.Append(string.IsNullOrWhiteSpace(attribute.Type)? "object": attribute.Type);
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
            }

            sb.Append(";");
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
