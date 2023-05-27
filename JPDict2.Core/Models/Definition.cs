using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models
{
    public class Definition
    {
        public int Id { get; set; }
        public string SummaryText { get; set; }
        public List<int> Senses { get; set; }
        public List<int> Kanjis { get; set; }
        public List<int> Kanas { get; set; }

        public Definition(int id, string summaryText, string senses, string kanjis, string kanas)
        {
            Id = id;
            SummaryText = summaryText;
            Senses = new List<int>();
            Kanjis = new List<int>();
            Kanas = new List<int>();

            senses.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => Senses.Add(int.Parse(x)));
            kanjis.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => Kanjis.Add(int.Parse(x)));
            kanas.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => Kanas.Add(int.Parse(x)));
        }
    }
}
