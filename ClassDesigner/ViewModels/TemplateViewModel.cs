using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClassDesigner.ViewModels
{
    public class TemplateViewModel
    {
        public TemplateViewModel(string path) 
        { 
            Path = path;
            XElement template = XElement.Load(path);
            Content = template;
            Name = template.Attribute("TemplateName").Value;
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public XElement Content { get; set; }
    }
}
