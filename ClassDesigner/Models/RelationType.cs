using System.ComponentModel;

namespace ClassDesigner.Models
{
    public enum RelationType
    {
        [Description("Ассоциация")]
        Association,
        [Description("Аггрегация")]
        Aggregation,
        [Description("Композиция")]
        Composition,
        [Description("Обобщение")]
        Generalization,
        [Description("Реализация")]
        Realization,
        [Description("Зависимость")]
        Dependency
    }
}
