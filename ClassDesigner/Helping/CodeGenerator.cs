using ClassDesigner.Helping.Serializers;
using ClassDesigner.Models;
using System.Collections.Generic;
using System.IO;

namespace ClassDesigner.Helping
{
    public class CodeGenerator
    {
        ISerializer Serializer;
        LanguagesList.Languages Language;

        public CodeGenerator(LanguagesList.Languages language)
        {
            this.Language = language;
            switch (language)
            {
                case LanguagesList.Languages.CSharp:
                    Serializer = new CSharpSerializer();
                    break;
                case LanguagesList.Languages.Java:
                    Serializer = new JavaSerializer();
                    break;
                case LanguagesList.Languages.Python:

                    break;
                case LanguagesList.Languages.Kotlin:
                    Serializer = new KotlinSerializer();
                    break;
            }

        }

        public void GenerateCode(IEnumerable<IEntry> entries, string path)
        {
            try
            {
                if (entries is null)
                {
                    MessageBox.MessageBox.Show("Ошибка", "В рабочей области нет сущностей для генерации кода", MessageBox.MessageBoxButtons.Ok);
                    return;
                }

                var dir = Path.Combine(path, Language.ToString());
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

                foreach (var item in entries)
                {
                    File.WriteAllText(Path.Combine(dir, $"{item.Name}.{Serializer.Extension}"), Serializer.SerializeEntry(item));
                }
                MessageBox.MessageBox.Show("Ошибка", "В рабочей области нет сущностей для генерации кода", MessageBox.MessageBoxButtons.Ok);
            }
            catch (System.Exception ex)
            {
                MessageBox.MessageBox.Show("Ошибка", ex.Message, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}
