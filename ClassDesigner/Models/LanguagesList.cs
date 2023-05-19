using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClassDesigner.Models
{
    public class LanguagesList : IEnumerable
    {
        public class Language
        {
            public Languages Name { get; set; }
            public bool IsSelected { get; set; }
        }

        public enum Languages
        {
            CSharp,
            Java,
            Python,
            Kotlin
        }

        public List<Language> LanguageList = new List<Language>
        {
            new Language(){ Name = Languages.CSharp, IsSelected = false },
            new Language(){ Name = Languages.Java, IsSelected = false },
            new Language(){ Name = Languages.Python, IsSelected = false },
            new Language(){ Name = Languages.Kotlin, IsSelected = false }
        };

        public bool IsAnySelected
        {
            get
            {
                return LanguageList.Any(x => x.IsSelected);
            }
        }

        public IEnumerator GetEnumerator() => LanguageList.GetEnumerator();
    }
}
