using System.ComponentModel;

namespace ClassDesigner.Models
{
    public enum Stereotype
    {
        [Description("Интерфейс")]
        Interface,
        [Description("Перечисление")]
        Enum,
        [Description("Структура")]
        Struct
    }
}
