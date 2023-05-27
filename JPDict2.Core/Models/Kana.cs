using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models
{
    public enum KanaAnnotation
    {
        Okurigana = 0,
        Archaism = 1,
        Rare = 2,
        IrregularKanaUsage = 3,
        Obsolete = 4,
    }

    public class Kana
    {
        public int Id { get; set; }
        public string KanaStr { get; set; }
        public List<KanaAnnotation> KanaAnnotations { get; set; }
        public string Romaji { get; set; } = "";

        public Kana(int id, string kanaStr, string kanaAnnotations)
        {
            Id = id;
            KanaStr = kanaStr;
            KanaAnnotations = new List<KanaAnnotation>();
            if (!string.IsNullOrEmpty(kanaAnnotations))
                kanaAnnotations.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => KanaAnnotations.Add((KanaAnnotation)int.Parse(x)));
        }
    }
}
