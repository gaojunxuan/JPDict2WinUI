using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models
{
    public enum KanjiAnnotation
    {
        Okurigana = 0,
        Archaism = 1,
        Rare = 2,
        IrregularKanjiUsage = 3,
        Ateji = 4,
        Obsolete = 5,
    }

    public class Kanji
    {
        public int Id { get; set; }
        public string KanjiStr { get; set; }
        public List<KanjiAnnotation> KanjiAnnotations { get; set; }

        public Kanji(int id, string kanjiStr, string kanjiAnnotations)
        {
            Id = id;
            KanjiStr = kanjiStr;
            KanjiAnnotations = new List<KanjiAnnotation>();
            if (!string.IsNullOrEmpty(kanjiAnnotations))
                kanjiAnnotations.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => KanjiAnnotations.Add((KanjiAnnotation)int.Parse(x)));
        }
    }
}
