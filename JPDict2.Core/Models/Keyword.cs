using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models;

public enum KeywordType
{
    Kanji = 0,
    Reading = 1,
    English = 2
}

public class Keyword
{
    public int Id { get; set; }
    public string Text { get; set; }
    public KeywordType KeywordType { get; set; }
    public int DefEntryId { get; set; }

    public Keyword(int id, string text, KeywordType keywordType, int defEntryId)
    {
        Id = id;
        Text = text;
        KeywordType = keywordType;
        DefEntryId = defEntryId;
    }
}
