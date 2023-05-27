using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models
{
    public class Sense
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> CrossReferences { get; set; }
        public string Pos { get; set; }
        public string Field { get; set; }
        public string Dialect { get; set; }
        public string EnglishExampleText { get; set; }
        public string JapaneseExampleText { get; set; }

        public Sense(int id, string text, string crossReferences, string pos, string field, string dialect, string englishExampleText, string japaneseExampleText)
        {
            Id = id;
            Text = text;
            CrossReferences = new List<string>();
            crossReferences.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => CrossReferences.Add(x));
            Pos = pos;
            Field = field;
            Dialect = dialect;
            EnglishExampleText = englishExampleText;
            JapaneseExampleText = japaneseExampleText;
        }
    }
}
