using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models;

public class DisplayDefinition: INotifyPropertyChanged
{
    public int Id
    {
        get; set;
    }
    public List<Sense> Senses
    {
        get; set;
    }
    public List<Kanji> Kanjis
    {
        get; set;
    }
    public List<Kana> Kanas
    {
        get; set;
    }

    public DisplayDefinition(int id)
    {
        Id = id;
        Senses = new List<Sense>();
        Kanjis = new List<Kanji>();
        Kanas = new List<Kana>();
        OnPropertyChanged(nameof(Senses));
    }

    public DisplayDefinition(int id, List<Sense> senses, List<Kanji> kanjis, List<Kana> kanas)
    {
        Id = id;
        Senses = senses;
        Kanjis = kanjis;
        Kanas = kanas;
        OnPropertyChanged(nameof(Senses));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
