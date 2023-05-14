using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public interface ISerializer
    {
        public string Extension { get; }
        string SerializeEntry(IEntry entry);
    }
}
