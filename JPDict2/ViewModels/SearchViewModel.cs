using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using JPDict2.Contracts.ViewModels;
using JPDict2.Core;
using JPDict2.Core.Contracts.Services;
using JPDict2.Core.Models;
using JPDict2.NLP;

namespace JPDict2.ViewModels;

public partial class SearchViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    [ObservableProperty]
    private SearchResult? selected;

    public ObservableCollection<SearchResult> SearchResults { get; private set; } = new ObservableCollection<SearchResult>();

    public ObservableCollection<Tuple<string, string, string, string, string>> SegmentedInput { get; private set; } = new ObservableCollection<Tuple<string, string, string, string, string>>();

    //public ObservableCollection<

    public SearchViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        
    }

    public void OnNavigatedFrom()
    {
    }

    public void EnsureItemSelected()
    {
        //Selected ??= SampleItems.First();
    }

    public async void UpdateSearchResults(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            SearchResults.Clear();
            return;
        }
        var results = await JmdictDbQueryEngine.Instance.GetSearchResults(keyword);
        int i = 0;
        foreach (var result in results)
        {
            if (i < SearchResults.Count)
            {
                SearchResults[i] = result;
             }
            else
            {
                SearchResults.Add(result);
            }
            i++;
        }
        while (i < SearchResults.Count)
        {
            SearchResults.RemoveAt(i);
        }
    }

    public async void NotifyItemSelected()
    {
        //DisplayDefinition = await JmdictDbQueryEngine.Instance.GetDisplayDefinitionAsync(Selected.Id);
    }

    public void UpdateSegmentedInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            SegmentedInput.Clear();
            return;
        }
        var lemmatized = Lemmatizer.Instance.GetLemmatized(input);
        int i = 0;
        foreach (var (surface, stem, reading, pos) in lemmatized)
        {
            if (i < SegmentedInput.Count)
            {
                string romaji = AlphabetConverter.Instance.ToRomaji(reading);
                SegmentedInput[i] = Tuple.Create(surface, stem, reading, pos, reading != "*" ? romaji : "");
            } 
            else
            {
                string romaji = AlphabetConverter.Instance.ToRomaji(reading);
                SegmentedInput.Add(Tuple.Create(surface, stem, reading, pos, reading != "*" ? romaji : ""));
            }
            i++;
        }
        // remove extra items
        while (i < SegmentedInput.Count)
        {
            SegmentedInput.RemoveAt(i);
        }
    }
}
