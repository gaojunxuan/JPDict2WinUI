using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models;
internal class SentenceOfTheDayEntry
{
    public int Id
    {
        get; set;
    }
    public string Sentence
    {
        get; set;
    }
    public string Translation
    {
        get; set;
    }
    public string Annotations
    {
        get; set;
    }
}
