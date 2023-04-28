using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDesigner.Helping
{
    public static class StringBuilderExtension
    {
        public static void Space(this StringBuilder sb, int count = 1) 
        { 
            for (int i = 0; i < count; i++)
            {
                sb.Append(" ");
            }
        }

        public static void Tab(this StringBuilder sb, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                sb.Space(4);
            }
        }
    }
}
