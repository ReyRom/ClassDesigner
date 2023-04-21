using System.ComponentModel;

namespace ClassDesigner.Models
{
    public enum VisibilityType
    {
        [Description("Публичный")]
        Public = '+',
        [Description("Приватный")]
        Private = '-',
        [Description("Защищенный")]
        Protected = '#',
        [Description("Пакет")]
        Internal = '~'
    }
}
