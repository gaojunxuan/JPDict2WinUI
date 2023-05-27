using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models;
public class KanjiEntry
{
    public int Id
    {
        get; set;
    }
    public string Kanji
    {
        get; set;
    }
    public int StrokeCount
    {
        get; set;
    }
    public List<int> RadicalIds
    {
        get; set;
    }
    public int Grade
    {
        get; set;
    }
    public int JLPTLevel
    {
        get; set;
    }
    public int Frequency
    {
        get; set;
    }
    public string OnReading
    {
        get; set;
    }
    public string KunReading
    {
        get; set;
    }
    public string Meanings
    {
        get; set;
    }
    public string RadicalLiterals
    {
        get; set;
    } = string.Empty;
    public List<KanjiGuideStep> KanjiWritingGuide
    {
        get; set; 
    } = new List<KanjiGuideStep>();
    
}
