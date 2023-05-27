using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using JPDict2.Core;
using JPDict2.Core.Models;
using JPDict2.Helpers;
using JPDict2.NLP;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace JPDict2.Views;

[ObservableRecipient, ObservableObject]
public sealed partial class SearchDetailControl : UserControl
{

    [ObservableProperty]
    private DisplayDefinition? displayDef = new DisplayDefinition(0);

    [ObservableProperty]
    private ObservableCollection<Tuple<string, List<Sense>>> groupedSenses = new ObservableCollection<Tuple<string, List<Sense>>>();

    [ObservableProperty]
    private ObservableCollection<KanjiEntry> kanjiCharacters = new ObservableCollection<KanjiEntry>();

    [ObservableProperty]
    private bool isVerb = false;

    [ObservableProperty]
    private ConjugationHelper.VerbConjugations verbConjugations = new ConjugationHelper.VerbConjugations();

    public SearchResult? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as SearchResult;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(SearchResult), typeof(SearchDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public SearchDetailControl()
    {
        InitializeComponent();
    }

    private static async void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SearchDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
            control.DisplayDef = await JmdictDbQueryEngine.Instance.GetDisplayDefinitionAsync((e.NewValue as SearchResult)!.Id);
            control.GroupedSenses = new ObservableCollection<Tuple<string, List<Sense>>>(control.DisplayDef.Senses.GroupBy(s => s.Pos).ToList().ConvertAll(t => new Tuple<string, List<Sense>>(t.Key, t.ToList())));

            control.Utils_GridView.Children.Clear();
            control.UpdateVerbConjugations();
            control.UpdateKanaRomaji();
            if (control.DisplayDef.Kanjis.Count > 0)
            {
                control.Utils_GridView.Children.Add(control.KanjiPanel);
                control.UpdateKanjiCharactersList();
            }
            if (control.IsVerb)
            {
                control.Utils_GridView.Children.Add(control.ConjugationPanel);
            }

        }
    }

    private async void UpdateKanjiCharactersList()
    {
        Regex reg = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        List<string> kanjiCharacters = new List<string>();
        foreach (var kanji in DisplayDef.Kanjis)
        {
            foreach (var c in kanji.KanjiStr)
            {
                if (reg.IsMatch(c.ToString()))
                {
                    if (!kanjiCharacters.Contains(c.ToString()))
                    {
                        kanjiCharacters.Add(c.ToString());
                    }
                }
            }
        }
        KanjiCharacters = new ObservableCollection<KanjiEntry>(await KanjiDictQueryEngine.Instance.GetKanjiEntriesAsync(kanjiCharacters));
        foreach (var kanji in KanjiCharacters)
        {
            kanji.RadicalLiterals = string.Join(", ", kanji.RadicalIds.ConvertAll(id => KanjiDictQueryEngine.ConvertRadicalIdToRadicalLiteral(id)));
            kanji.KanjiWritingGuide = await KanjiDictQueryEngine.Instance.GetKanjiWritingGuideAsync(kanji.Kanji);
        }
    }

    public async void UpdateKanaRomaji()
    {
        foreach (var kana in DisplayDef.Kanas)
        {
            kana.Romaji = string.Join(" ", AlphabetConverter.Instance.HiraganaToRomajiList(kana.KanaStr));

        }
    }

    private void ShowHandwritingButton_Click(object sender, RoutedEventArgs e)
    {
        ((sender as FrameworkElement).FindResource("HandwritingGuide") as TeachingTip).IsOpen = true;
    }

    private void KanjiSvg_Loaded(object sender, RoutedEventArgs e)
    {
        var transform = new ScaleTransform();
        transform.ScaleX = 0.4;
        transform.ScaleY = 0.4;
        (sender as Microsoft.UI.Xaml.Shapes.Path).Data.Transform = transform;
    }

    private void UpdateVerbConjugations()
    {
        string posStr = "";
        // concatenate all pos description
        foreach (var s in DisplayDef.Senses)
        {
            posStr += s.Pos + ", ";
        }

        // first obtain a list of conjugatable pos
        List<ConjugationHelper.ConjugatablePartOfSpeech> pos = ConjugationHelper.IdentifyConjugatablePos(posStr);

        // then obtain a list of conjugations
        if (ConjugationHelper.IsAnyVerb(pos))
        {
            IsVerb = true;
            string keyword = DisplayDef.Kanas[0].KanaStr;
            ConjugationHelper.VerbConjugations verbConjugations = new ConjugationHelper.VerbConjugations()
            {
                Causative = ConjugationHelper.GetCausative(keyword, pos),
                EbaForm = ConjugationHelper.GetEba(keyword, pos),
                Imperative = ConjugationHelper.GetImperative(keyword, pos),
                MasuForm = ConjugationHelper.GetMasu(keyword, pos),
                MasuNegative = ConjugationHelper.GetNegativeMasen(keyword, pos),
                NegativeCausative = ConjugationHelper.GetNegativeCausative(keyword, pos),
                NegativeForm = ConjugationHelper.GetNegative(keyword, pos),
                NegativeImperative = ConjugationHelper.GetNegativeImperative(keyword, pos),
                NegativePassive = ConjugationHelper.GetNegativePassive(keyword, pos),
                NegativePotential = ConjugationHelper.GetNegativePotential(keyword, pos),
                OriginalForm = keyword,
                Passive = ConjugationHelper.GetPassive(keyword, pos),
                PastNegative = ConjugationHelper.GetPastNegative(keyword, pos),
                Potential = ConjugationHelper.GetPotential(keyword, pos),
                TaForm = ConjugationHelper.GetTaForm(keyword, pos),
                TeForm = ConjugationHelper.GetTeForm(keyword, pos),
                Volitional = ConjugationHelper.GetVolitional(keyword, pos)
            };
            VerbConjugations = verbConjugations;
        }
        else
        {
            IsVerb = false;
        }
        
    }
}
