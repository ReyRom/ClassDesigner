using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Models
{
    public enum EntityType
    {
        [Description("Не установлено")]
        None,
        [Description("Класс")]
        Class,
        [Description("Соединительная линия")]
        Connection,
        [Description("Аттрибут")]
        Attribute,
        [Description("Метод")]
        Method
    }
}
