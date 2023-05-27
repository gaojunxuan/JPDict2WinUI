using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models;
public class KanjiGuideStep
{
    public string Kanji
    {
        get; set;
    }
    public string SVGPath
    {
        get; set;
    }
    public int Order
    {
        get; set;
    }
}
