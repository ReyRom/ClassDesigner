using System.Collections;
using System.Collections.Generic;

namespace ClassDesigner.Models
{
    public class LanguagesList : IEnumerable
    {
        public class Language
        {
            public string Name { get; set; }
            public bool IsSelected { get; set; }
        }

        public List<Language> Languages = new List<Language>
        {
            new Language(){ Name = "C#", IsSelected = false },
            new Language(){ Name = "Java", IsSelected = false },
            new Language(){ Name = "Kotlin", IsSelected = false },
            new Language(){ Name = "Python", IsSelected = false }
        };

        public IEnumerator GetEnumerator() => Languages.GetEnumerator();
    }
}
