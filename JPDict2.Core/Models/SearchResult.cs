using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPDict2.Core.Extensions;

namespace JPDict2.Core.Models;

public class SearchResult
{
    public int Id { get; set; }
    public string PrimaryField { get; set; }
    public string SecondaryField { get; set; }
    public string Romaji { get; set; }
    public string DefinitionSummary { get; set; }
    public string FontFamily
    {
        get
        {
            if (PrimaryField.ContainsUnicodeCharacter())
            {
                return "Assets/UDDigiKyokashoN-R# UD Digi Kyokasho N-R";
            }
            else
            {
                return "Segoe UI";
            }
        }
    }

    public SearchResult(int id, string primary, string secondary, string romaji, string definitionSummary)
    {
        Id = id;
        PrimaryField = primary;
        SecondaryField = secondary;
        Romaji = romaji;
        DefinitionSummary = definitionSummary;
    }
}
