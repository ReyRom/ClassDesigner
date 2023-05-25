using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ClassDesigner.Models
{
    public class LanguagesList : IEnumerable
    {
        public class Language : INotifyPropertyChanged
        {
            private bool? isSuccess;

            public Languages Name { get; set; }
            public bool IsSelected { get; set; }

            public bool? IsSuccess
            {
                get => isSuccess; set
                {
                    isSuccess = value;
                    OnPropertyChanged(nameof(IsSuccess));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
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
            new Language(){ Name = Languages.CSharp, IsSelected = false, IsSuccess = null },
            new Language(){ Name = Languages.Java, IsSelected = false, IsSuccess = null },
            new Language(){ Name = Languages.Python, IsSelected = false, IsSuccess = null},
            new Language(){ Name = Languages.Kotlin, IsSelected = false, IsSuccess = null }
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
