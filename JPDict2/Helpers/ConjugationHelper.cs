using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JPDict2.Helpers
{
    public class ConjugationHelper
    {
        public enum ConjugatablePartOfSpeech
        {
            // Unspecified
            Unspecified,
            // Keiyoushi
            IAdjective,
            // Keiyoushi (yoi/ii)
            YoiAdjective,
            // Keiyoushi (kari)
            KariAdjective,
            // Keiyoushi (ku)
            KuAdjective,
            // Keiyoudoushi
            NaAdjective,
            // Keiyoudoushi (nari)
            NariAdjective,
            // Keiyoushi (shiku)
            ShikuAdjective,
            // Taru Adjective
            TaruAdjective,
            // Ichidan verb
            IchidanVerb,
            // Ichidan verb (kureru)
            KureruIchidanVerb,
            // Ichidan verb (zuru)
            ZuruIchidanVerb,
            // Godan verb
            GodanVerb,
            // Godan verb (aru)
            AruGodanVerb,
            // Godan verb (iku/yuku)
            IkuGodanVerb,
            // Godan verb (ru irregular)
            RuIrregularGodanVerb,
            // Godan verb (u irregular, e.g. 問う, a.k.a. う音便)
            UIrregularGodanVerb,
            // Godan verb (uru)
            UruGodanVerb,
            // Nu verb
            NaHenVerb,
            // Su verb
            SuVerb,
            // Suru verb
            SaHenVerb,
            // Suru verb (stem only)
            SaHenVerbStemOnly,
            // Special suru verb
            SpecialSuru,
            // Kuru verb
            KaHenVerb,
            // Ri verb (ラ変 e.g. あり, をり, 侍り, いまそかり)
            RaHenVerb,
        }

        public readonly static Dictionary<string, ConjugatablePartOfSpeech> PosLabelToConjugatablePartOfSpeechMapping = new Dictionary<string, ConjugatablePartOfSpeech>()
        {
            { "adj-i", ConjugatablePartOfSpeech.IAdjective },
            { "adj-ix", ConjugatablePartOfSpeech.YoiAdjective },
            { "adj-kari", ConjugatablePartOfSpeech.KariAdjective },
            { "adj-ku", ConjugatablePartOfSpeech.KuAdjective },
            { "adj-na", ConjugatablePartOfSpeech.NaAdjective },
            { "adj-nari", ConjugatablePartOfSpeech.NariAdjective },
            { "adj-shiku", ConjugatablePartOfSpeech.ShikuAdjective },
            { "adj-t", ConjugatablePartOfSpeech.TaruAdjective },
            { "v1", ConjugatablePartOfSpeech.IchidanVerb },
            { "v1-s", ConjugatablePartOfSpeech.KureruIchidanVerb },
            { "vz", ConjugatablePartOfSpeech.ZuruIchidanVerb },
            { "v5", ConjugatablePartOfSpeech.GodanVerb },
            { "v5aru", ConjugatablePartOfSpeech.AruGodanVerb },
            { "v5b", ConjugatablePartOfSpeech.GodanVerb },
            { "v5g", ConjugatablePartOfSpeech.GodanVerb },
            { "v5k", ConjugatablePartOfSpeech.GodanVerb },
            { "v5k-s", ConjugatablePartOfSpeech.IkuGodanVerb },
            { "v5m", ConjugatablePartOfSpeech.GodanVerb },
            { "v5n", ConjugatablePartOfSpeech.GodanVerb },
            { "v5r", ConjugatablePartOfSpeech.GodanVerb },
            { "v5r-i", ConjugatablePartOfSpeech.RuIrregularGodanVerb },
            { "v5s", ConjugatablePartOfSpeech.GodanVerb },
            { "v5t", ConjugatablePartOfSpeech.GodanVerb },
            { "v5u", ConjugatablePartOfSpeech.GodanVerb },
            { "v5u-s", ConjugatablePartOfSpeech.UIrregularGodanVerb },
            { "v5uru", ConjugatablePartOfSpeech.UruGodanVerb },
            { "v5z", ConjugatablePartOfSpeech.GodanVerb },
            { "vk", ConjugatablePartOfSpeech.KaHenVerb },
            { "vn", ConjugatablePartOfSpeech.NaHenVerb },
            { "vs", ConjugatablePartOfSpeech.SaHenVerbStemOnly },
            { "vs-c", ConjugatablePartOfSpeech.SuVerb },
            { "vs-s", ConjugatablePartOfSpeech.SpecialSuru },
            { "vs-i", ConjugatablePartOfSpeech.SaHenVerb },
            { "vr", ConjugatablePartOfSpeech.RaHenVerb },
        };

        static readonly Dictionary<string, string> PosDescriptionToLabel = new Dictionary<string, string>()
        {
            { "adjective (keiyoushi)", "adj-i" },
            { "adjective (keiyoushi) - yoi/ii class", "adj-ix" },
            { "'kari' adjective (archaic)", "adj-kari" },
            { "'ku' adjective (archaic)", "adj-ku" },
            { "adjectival nouns or quasi-adjectives (keiyodoshi)", "adj-na" },
            { "archaic/formal form of na-adjective", "adj-nari" },
            { "'shiku' adjective (archaic)", "adj-shiku" },
            { "'taru' adjective", "adj-t" },
            { "Ichidan verb", "v1" },
            { "Ichidan verb - kureru special class", "v1-s" },
            { "Godan verb - -aru special class", "v5aru" },
            { "Godan verb with 'bu' ending", "v5b" },
            { "Godan verb with 'gu' ending", "v5g" },
            { "Godan verb with 'ku' ending", "v5k" },
            { "Godan verb - Iku/Yuku special class", "v5k-s" },
            { "Godan verb with 'mu' ending", "v5m" },
            { "Godan verb with 'nu' ending", "v5n" },
            { "Godan verb with 'ru' ending", "v5r" },
            { "Godan verb with 'ru' ending (irregular verb)", "v5r-i" },
            { "Godan verb with 'su' ending", "v5s" },
            { "Godan verb with 'tsu' ending", "v5t" },
            { "Godan verb with 'u' ending", "v5u" },
            { "Godan verb with 'u' ending (special class)", "v5u-s" },
            { "Godan verb - Uru old class verb (old form of Eru)", "v5uru" },
            { "Kuru verb - special class", "vk" },
            { "irregular nu verb", "vn" },
            { "irregular ru verb, plain form ends with -ri", "vr" },
            { "noun or participle which takes the aux. verb suru", "vs" },
            { "su verb - precursor to the modern suru", "vs-c" },
            { "suru verb - included", "vs-i" },
            { "suru verb - special class", "vs-s" },
            { "Ichidan verb - zuru verb (alternative form of -jiru verbs)", "vz" }
        };

        static readonly List<string> hira = new List<string>() { "わ", "い", "う", "え", "お", 
                                                        "か", "き", "く", "け", "こ", 
                                                        "さ", "し", "す", "せ", "そ", 
                                                        "た", "ち", "つ", "て", "と", 
                                                        "な", "に", "ぬ", "ね", "の", 
                                                        "は", "ひ", "ふ", "へ", "ほ", 
                                                        "ま", "み", "む", "め", "も", 
                                                        "や", "い", "ゆ", "え", "よ", 
                                                        "ら", "り", "る", "れ", "ろ", 
                                                        "わ", "い", "う", "え", "を", 
                                                        "ん", 
                                                        "が", "ぎ", "ぐ", "げ", "ご", 
                                                        "ざ", "じ", "ず", "ぜ", "ぞ", 
                                                        "だ", "ぢ", "づ", "で", "ど", 
                                                        "ば", "び", "ぶ", "べ", "ぼ", 
                                                        "ぱ", "ぴ", "ぷ", "ぺ", "ぽ" 
                                                     };

        public static List<ConjugatablePartOfSpeech> IdentifyConjugatablePos(string pos)
        {
            List<ConjugatablePartOfSpeech> conjugatablePos = new List<ConjugatablePartOfSpeech>();
            foreach (var posDescription in PosDescriptionToLabel)
            {
                if (pos.Contains(posDescription.Key))
                {
                    if (PosLabelToConjugatablePartOfSpeechMapping.ContainsKey(posDescription.Value))
                    conjugatablePos.Add(PosLabelToConjugatablePartOfSpeechMapping[posDescription.Value]);
                }
            }
            return conjugatablePos;
        }

        public class VerbConjugations
        {
            public string OriginalForm
            {
                get; set;
            }
            public string MasuForm
            {
                get; set;
            }
            public string MasuNegative
            {
                get; set;
            }
            public string Causative
            {
                get; set;
            }
            public string NegativeCausative
            {
                get; set;
            }
            public string EbaForm
            {
                get; set;
            }
            public string Imperative
            {
                get; set;
            }
            public string NegativeImperative
            {
                get; set;
            }
            public string NegativeForm
            {
                get; set;
            }
            public string PastNegative
            {
                get; set;
            }
            public string Passive
            {
                get; set;
            }
            public string NegativePassive
            {
                get; set;
            }
            public string Potential
            {
                get; set;
            }
            public string NegativePotential
            {
                get; set;
            }
            public string TaForm
            {
                get; set;
            }
            public string TeForm
            {
                get; set;
            }
            public string Volitional
            {
                get; set;
            }
        }

        public class AdjectiveConjugations
        {
            public string PresentForm
            {
                get; set;
            }
            public string PastForm
            {
                get; set;
            }
            public string TeForm
            {
                get; set;
            }
            public string EbaForm
            {
                get; set;
            }
            public string TaraForm
            {
                get; set;
            }
            public string Volitional
            {
                get; set;
            }
            public string NegativePresentForm
            {
                get; set;
            }
            public string NegativePastForm
            {
                get; set;
            }
        }

        private static bool IsRegularGodan(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.GodanVerb);
        }

        private static bool IsSpecialGodanIku(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.IkuGodanVerb);
        }

        private static bool IsSpecialGodanU(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.UIrregularGodanVerb);
        }

        private static bool IsSpecialGodanUru(List<ConjugatablePartOfSpeech> pos)
        {

            return pos.Contains(ConjugatablePartOfSpeech.UruGodanVerb);
        }

        private static bool IsSpecialGodanAru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.AruGodanVerb);
        }

        private static bool IsSpecialGodanRu(List<ConjugatablePartOfSpeech> pos)
        {
        
            return pos.Contains(ConjugatablePartOfSpeech.RuIrregularGodanVerb);
        }

        private static bool IsAnyGodan(List<ConjugatablePartOfSpeech> pos)
        {
        
            return IsRegularGodan(pos) || IsSpecialGodanIku(pos) || IsSpecialGodanU(pos) || IsSpecialGodanUru(pos) || IsSpecialGodanAru(pos);
        }

        private static bool IsRegularIchidan(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.IchidanVerb);
        }

        private static bool IsSpecialIchidanKureru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.KureruIchidanVerb);
        }

        private static bool IsSpecialIchidanZuru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.ZuruIchidanVerb);
        }

        private static bool IsAnyIchidan(List<ConjugatablePartOfSpeech> pos)
        {
            return IsRegularIchidan(pos) || IsSpecialIchidanKureru(pos) || IsSpecialIchidanZuru(pos);
        }

        private static bool IsRegularSuru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.SaHenVerb);
        }

        private static bool IsRegularSuruWithStemOnly(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.SaHenVerbStemOnly);
        }

        public static bool IsSpecialSuru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.SpecialSuru);
        }

        public static bool IsSpecialSingleCharSuru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.SpecialSuru);
        }

        private static bool IsRegularKuru(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.KaHenVerb);
        }

        private static bool IsSpecialNu(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.NaHenVerb);
        }

        private static bool IsSpecialRi(List<ConjugatablePartOfSpeech> pos)
        {
            return pos.Contains(ConjugatablePartOfSpeech.RaHenVerb);
        }

        public static bool IsAnyVerb(List<ConjugatablePartOfSpeech> pos)
        {
            return IsAnyGodan(pos) || IsAnyIchidan(pos) || IsRegularSuru(pos) || IsSpecialSuru(pos) || IsRegularKuru(pos) || IsSpecialNu(pos) || IsSpecialRi(pos);
        }

        static string ShiftToA(string word)
        {
            string okuri = word.Substring(word.Length - 1);
            okuri = hira[hira.IndexOf(okuri) - 2];
            return word.Substring(0, word.Length - 1) + okuri;
        }

        static string ShiftToI(string word)
        {
            string okuri = word.Substring(word.Length - 1);
            okuri = hira[hira.IndexOf(okuri) - 1];
            return word.Substring(0, word.Length - 1) + okuri;
        }
        static string ShiftToE(string word)
        {
            string okuri = word.Substring(word.Length - 1);
            okuri = hira[hira.IndexOf(okuri) + 1];
            return word.Substring(0, word.Length - 1) + okuri;
        }
        static string ShiftToO(string word)
        {
            string okuri = word.Substring(word.Length - 1);
            okuri = hira[hira.IndexOf(okuri) + 2];
            return word.Substring(0, word.Length - 1) + okuri;
        }

        static (string, List<ConjugatablePartOfSpeech>) PrepSpecialZuru(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var newConjugatablePartOfSpeeches = new List<ConjugatablePartOfSpeech>();
            bool found = false;
            if (IsSpecialIchidanZuru(conjugatablePartOfSpeeches))
            {
                for (int i = 0; i < conjugatablePartOfSpeeches.Count; i++)
                {
                    if (conjugatablePartOfSpeeches[i] == ConjugatablePartOfSpeech.ZuruIchidanVerb)
                    {
                        newConjugatablePartOfSpeeches.Add(ConjugatablePartOfSpeech.IchidanVerb);
                        found = true;
                    }
                    else
                    {
                        newConjugatablePartOfSpeeches.Add(conjugatablePartOfSpeeches[i]);
                    }

                }
                return (word.Replace("ずる", "じる"), newConjugatablePartOfSpeeches);
            }
            else
            {
                return (word, conjugatablePartOfSpeeches);
            }
            
        }

        static (string, List<ConjugatablePartOfSpeech>) PrepSpecialRi(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var newConjugatablePartOfSpeeches = new List<ConjugatablePartOfSpeech>();
            if (IsSpecialRi(conjugatablePartOfSpeeches))
            {
                for (int i = 0; i < conjugatablePartOfSpeeches.Count; i++)
                {
                    if (conjugatablePartOfSpeeches[i] == ConjugatablePartOfSpeech.RaHenVerb)
                    {
                        newConjugatablePartOfSpeeches.Add(ConjugatablePartOfSpeech.RuIrregularGodanVerb);
                    }
                    else
                    {
                        newConjugatablePartOfSpeeches.Add(conjugatablePartOfSpeeches[i]);
                    }
                }
                return (word.Substring(0, word.Length - 1) + "る", newConjugatablePartOfSpeeches);
            }
            else
            {
                return (word, conjugatablePartOfSpeeches);
            }
        }

        static (string, List<ConjugatablePartOfSpeech>) PrepSpecialSingleCharSuru(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var newConjugatablePartOfSpeeches = new List<ConjugatablePartOfSpeech>();
            if (IsSpecialSingleCharSuru(conjugatablePartOfSpeeches))
            {
                for (int i = 0; i < conjugatablePartOfSpeeches.Count; i++)
                {
                    if (conjugatablePartOfSpeeches[i] == ConjugatablePartOfSpeech.SpecialSuru)
                    {
                        newConjugatablePartOfSpeeches.Add(ConjugatablePartOfSpeech.GodanVerb);
                    }
                    else
                    {
                        newConjugatablePartOfSpeeches.Add(conjugatablePartOfSpeeches[i]);
                    }
                }
                return (word.Substring(0, word.Length - 1), newConjugatablePartOfSpeeches);
            }
            else
            {
                return (word, conjugatablePartOfSpeeches);
            }
            
        }

        static string PrepGodanVerbPhoneticChange(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            string newWord = word.Substring(0, word.Length - 1);
            if (word[word.Length - 1] == 'う' || word[word.Length - 1] == 'る' || word[word.Length - 1] == 'つ')
            {
                if (IsSpecialGodanU(conjugatablePartOfSpeeches))
                    newWord = newWord + "う";
                else
                    newWord = newWord + "っ";
            }
            else if (word[word.Length - 1] == 'ぬ' || word[word.Length - 1] == 'む' || word[word.Length - 1] == 'ぶ')
            {
                // nasal sound shift
                newWord = newWord + "ん";
            }
            else if (word[word.Length - 1] == 'く')
            {
                if (IsSpecialGodanIku(conjugatablePartOfSpeeches))
                {
                    newWord = newWord + "っ";
                }
                else
                {
                    newWord = newWord + "い";
                }
            }
            else if (word[word.Length - 1] == 'ぐ')
            {
                newWord = newWord + "い";
            }
            else
            {
                newWord = newWord + "し";
            }
            return newWord;
        }

        static string PrepMasuForm(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            word = word.Replace(" ", "").Replace("　", "");
            if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "ま";
            }
            else if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "きま";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                    return "しま";
                return word.Substring(0, word.Length - 2) + "しま";
            }
            else if (IsSpecialRi(conjugatablePartOfSpeeches))
            {
                return word + "ま";
            }
            else
            {
                if (IsSpecialGodanAru(conjugatablePartOfSpeeches))
                {
                    return word.Substring(0, word.Length - 1) + "いま";
                }
                // regular godan, na-hen
                return ShiftToI(word) + "ま";
            }
        }

        static string PrepTeTa(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            return PrepGodanVerbPhoneticChange(word, conjugatablePartOfSpeeches);
        }

        static string PrepNegative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "こな";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "しな";
                }
                else
                {
                    return word.Substring(0, word.Length - 2) + "しな";
                }
            }
            else if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "な";
            }
            else
            {
                return ShiftToA(word) + "な";
            }
        }

        static string PrepPotential(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "こられ";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "でき";
                }
                else
                {
                    return word.Substring(0, word.Length - 2) + "でき";
                }
            }
            else if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "られ";
            }
            else
            {
                return ShiftToE(word);
            }
        }

        static string PrepPassive(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "こられ";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "され";
                }
                else
                {
                    return word.Substring(0, word.Length - 2) + "され";
                }
            }
            else if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "られ";
            }
            else
            {
                return ShiftToA(word) + "れ";
            }
        }

        static string PrepImperative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "こい";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "しろ  (せよ)";
                }
                else
                {
                    return word.Substring(0, word.Length - 2) + "しろ  (せよ)";
                }
            }
            else if (IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1);
            }
            else if (IsRegularIchidan(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "ろ";
            }
            else
            {
                if (IsSpecialGodanAru(conjugatablePartOfSpeeches))
                {
                    return word.Substring(0, word.Length - 1) + "い";
                }
                return ShiftToE(word);
            }
        }

        static string PrepCausative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "こられ";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "させ";
                }
                else
                {
                    return word.Substring(0, word.Length - 2) + "させ";
                }
            }
            else if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "させ";
            }
            else
            {
                return ShiftToA(word) + "せ";
            }
        }

        public static string GetTeForm(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "て";
            }
            else if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "きて";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "して";
                }
                return word.Substring(0, word.Length - 2) + "して";
            }
            else
            {
                var newword = PrepGodanVerbPhoneticChange(word, conjugatablePartOfSpeeches);
                if (word[word.Length - 1] == 'ぐ' || word[word.Length - 1] == 'ぬ' || word[word.Length - 1] == 'む' || word[word.Length - 1] == 'ぶ')
                {
                    return newword + "で";
                }
                else
                {
                    return newword + "て";
                }
            }
        }

        public static string GetTaForm(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsRegularIchidan(conjugatablePartOfSpeeches) || IsSpecialIchidanKureru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 1) + "た";
            }
            else if (IsRegularKuru(conjugatablePartOfSpeeches))
            {
                return word.Substring(0, word.Length - 2) + "きた";
            }
            else if (IsRegularSuru(conjugatablePartOfSpeeches) || IsSpecialSuru(conjugatablePartOfSpeeches))
            {
                if (word == "する" || word == "為る")
                {
                    return "した";
                }
                return word.Substring(0, word.Length - 2) + "した";
            }
            else
            {
                var newword = PrepGodanVerbPhoneticChange(word, conjugatablePartOfSpeeches);
                if (word[word.Length - 1] == 'ぐ' || word[word.Length - 1] == 'ぬ' || word[word.Length - 1] == 'む' || word[word.Length - 1] == 'ぶ')
                {
                    return newword + "だ";
                }
                else
                {
                    return newword + "た";
                }
            }
        }

        public static string GetNegative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos)  = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            if (IsSpecialGodanRu(newPos))
            {
                // TODO: there can be other words ending with -aru
                return newWord.Substring(0, newWord.Length - 2) + "ない";
            }
            else if (IsSpecialRi(conjugatablePartOfSpeeches))
            {
                // -zu
                return PrepNegative(PrepSpecialRi(newWord, newPos).Item1, newPos) + "ず";
            }
            else
                return PrepNegative(newWord, newPos) + "い";
        }

        public static string GetPastNegative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            if (IsSpecialGodanRu(newPos))
            {
                // TODO: there can be other words ending with -aru
                return newWord.Substring(0, newWord.Length - 2) + "なかった";
            }
            else if (IsSpecialRi(conjugatablePartOfSpeeches))
            {
                // -zu
                return PrepNegative(PrepSpecialZuru(newWord, newPos).Item1, newPos) + "ざりき/けり";
            }
            else
                return PrepNegative(newWord, newPos) + "い";
        }

        public static string GetEba(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);

            if (IsRegularKuru(newPos))
            {
                return newWord.Substring(0, newWord.Length - 2) + "くれば";
            }
            else if (IsRegularSuru(newPos) || IsSpecialSuru(newPos))
            {
                return newWord.Substring(0, newWord.Length - 1) + "れば";
            }
            else if (IsRegularIchidan(newPos) || IsSpecialIchidanKureru(newPos))
            {
                return newWord.Substring(0, newWord.Length - 1) + "れば";
            }
            else
            {
                // TODO: incorrect conversion for aru (correct is areba)
                return ShiftToE(newWord) + "ば";
            }
        }

        public static string GetPotential(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsSpecialGodanRu(conjugatablePartOfSpeeches)) {
                return word.Substring(0, word.Length - 2) + "ありえる";
            }
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepPotential(newWord, newPos) + "る";
        }

        public static string GetNegativePotential(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsSpecialGodanRu(conjugatablePartOfSpeeches))
            {
                // TODO: there can be other words ending with -aru
                return word.Substring(0, word.Length - 2) + "ありえない";
            }
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepPotential(newWord, newPos) + "ない";
        }

        public static string GetPassive(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsSpecialGodanRu(conjugatablePartOfSpeeches))
            {
                return "";
            }
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepPassive(newWord, newPos) + "る";
        }

        public static string GetNegativePassive(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            if (IsSpecialGodanRu(conjugatablePartOfSpeeches))
            {
                return "";
            }
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepPassive(newWord, newPos) + "ない";
        }

        public static string GetCausative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepCausative(newWord, newPos) + "る";
        }

        public static string GetNegativeCausative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepCausative(newWord, newPos) + "ない";
        }

        public static string GetImperative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepImperative(newWord, newPos);
        }

        public static string GetNegativeImperative(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, _) = PrepSpecialRi(newWord, newPos);
            return newWord + "な";
        }

        public static string GetVolitional(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            if (IsRegularIchidan(newPos))
            {
                return word.Substring(0, word.Length - 1) + "よう";
            }
            else if (IsRegularIchidan(newPos))
            {
                return newWord.Substring(0, newWord.Length - 2) + "こよう";
            }
            else if (IsRegularIchidan(newPos))
            {
                if (word == "為る" || word == "する")
                {
                    return "しよう(そう)";
                }
                else
                {
                    return word.Substring(0, word.Length - 2) + "しよう(そう)";
                }
            }
            else
            {
                return ShiftToO(newWord) + "う";
            }
        }

        public static string GetMasu(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepMasuForm(newWord, newPos) + "す";
        }

        public static string GetNegativeMasen(string word, List<ConjugatablePartOfSpeech> conjugatablePartOfSpeeches)
        {
            var (newWord, newPos) = PrepSpecialZuru(word, conjugatablePartOfSpeeches);
            (newWord, newPos) = PrepSpecialSingleCharSuru(newWord, newPos);
            (newWord, newPos) = PrepSpecialRi(newWord, newPos);
            return PrepMasuForm(newWord, newPos) + "せん";
        }
    }
}
