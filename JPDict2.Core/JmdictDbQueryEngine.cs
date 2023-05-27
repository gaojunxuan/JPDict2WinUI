using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPDict2.Core.Extensions;
using JPDict2.Core.Models;
using Microsoft.Data.Sqlite;
using Windows.Management.Core;
using Windows.Storage;

namespace JPDict2.Core;

public class JmdictDbQueryEngine
{
    private static JmdictDbQueryEngine _instance;
    private static readonly object _lock = new object();

    public static JmdictDbQueryEngine Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new JmdictDbQueryEngine();
                return _instance;
            }
        }
        
    }

    private bool IsConnected
    {
        get;
        set;
    }

    SqliteConnection sqliteConnection;

    private JmdictDbQueryEngine()
    {
    }

    public async void InitializeDatabase()
    {
        bool isDatabaseExisting = true;
        if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("jmdict.db") == null)
        {
            isDatabaseExisting = false;
        }
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path,
                                     "jmdict.db");
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
        sqliteConnection = new SqliteConnection($"Filename={dbpath}");
        if (isDatabaseExisting)
        {
            sqliteConnection.Open();
            IsConnected = true;
            return;
        }
    }

    public void CloseDatabaseConnection()
    {
        if (IsConnected)
        {
            sqliteConnection.Close();
            sqliteConnection.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
            
        IsConnected = false;
    }

    public static async void CopyMainDatabaseFile()
    {
        var dbFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/jmdict.db"));
        await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, "jmdict.db", NameCollisionOption.ReplaceExisting);
    }

    const string KEYWORD_EXACT_QUERY = "SELECT * FROM keyword WHERE text = @keyword";
    const string KEYWORD_QUERY = "SELECT * FROM keyword WHERE text LIKE @prefix OR text LIKE @suffix OR text LIKE @keywordFuzzy";

    public async Task<List<Keyword>> SearchKeyword(string keyword)
    {
        SqliteCommand searchExactKeywordCmd = new SqliteCommand(KEYWORD_EXACT_QUERY, sqliteConnection);
        SqliteCommand searchKeyWordCmd = new SqliteCommand(KEYWORD_QUERY, sqliteConnection);
        if (keyword.Length < 2 && !ContainsUnicodeCharacter(keyword))
        {
            searchKeyWordCmd.Parameters.AddWithValue("@suffix", "");
            searchKeyWordCmd.Parameters.AddWithValue("@prefix", "");
            searchKeyWordCmd.Parameters.AddWithValue("@keywordFuzzy", "");
        }
        else if (ContainsUnicodeCharacter(keyword))
        {
            searchKeyWordCmd.Parameters.AddWithValue("@suffix", "");
            searchKeyWordCmd.Parameters.AddWithValue("@prefix", keyword + "_%");
            searchKeyWordCmd.Parameters.AddWithValue("@keywordFuzzy", "");
        }
        else
        {
            searchKeyWordCmd.Parameters.AddWithValue("@suffix", "");
            searchKeyWordCmd.Parameters.AddWithValue("@prefix", keyword + "_%");
            searchKeyWordCmd.Parameters.AddWithValue("@keywordFuzzy", "% " + keyword + " %");
        }
        searchExactKeywordCmd.Parameters.AddWithValue("@keyword", keyword);

        var result = await searchKeyWordCmd.ExecuteReaderAsync();
        var exactResult = await searchExactKeywordCmd.ExecuteReaderAsync();

        List<Keyword> keywords = new List<Keyword>();
        int count = 0;
        while (exactResult.Read())
        {
            keywords.Add(new Keyword(exactResult.GetInt32(0), exactResult.SafeGetString(1), (KeywordType)exactResult.GetInt32(2), exactResult.GetInt32(3)));
            count++;
            if (count > 50)
                break;
        }
        while (result.Read())
        {
            keywords.Add(new Keyword(result.GetInt32(0), result.SafeGetString(1), (KeywordType)result.GetInt32(2), result.GetInt32(3)));
            count++;
            if (count > 50)
                break;
        }
        await searchKeyWordCmd.DisposeAsync();
        return keywords;
    }

    const string DEFINITION_QUERY = "SELECT * FROM definition WHERE id = @id";
    public async Task<Definition> GetDefinitionAsync(int id)
    {
        SqliteCommand searchKeyWordCmd = new SqliteCommand(DEFINITION_QUERY, sqliteConnection);
        searchKeyWordCmd.Parameters.AddWithValue("@id", id);
        var result = await searchKeyWordCmd.ExecuteReaderAsync();
        Definition definition = null;
        if (result.HasRows)
        {
            result.Read();
            definition = new Definition(result.GetInt32(0), result.SafeGetString(1), result.SafeGetString(2), result.SafeGetString(3), result.SafeGetString(4));
        }
        await searchKeyWordCmd.DisposeAsync();
        return definition;
    }

    private bool ContainsUnicodeCharacter(string input)
    {
        const int MaxAnsiCode = 255;
        return input.Any(c => c > MaxAnsiCode);
    }

    const string KANA_QUERY = "SELECT * FROM kana WHERE id = @id";
    public async Task<Kana> GetKanaAsync(int id)
    {
        SqliteCommand searchKanaCmd = new SqliteCommand(KANA_QUERY, sqliteConnection);
        searchKanaCmd.Parameters.AddWithValue("@id", id);
        var result = await searchKanaCmd.ExecuteReaderAsync();
        Kana kana = null;
        if (result.HasRows)
        {
            result.Read();
            kana = new Kana(result.GetInt32(0), result.SafeGetString(1), result.SafeGetString(2));
        }
        await searchKanaCmd.DisposeAsync();
        return kana;
    }

    public async Task<List<Kana>> GetKanasAsync(List<int> ids)
    {
        List<Kana> kanas = new List<Kana>();
        foreach (var id in ids)
        {
            kanas.Add(await GetKanaAsync(id));
        }
        return kanas;
    }

    const string KANJI_QUERY = "SELECT * FROM kanji WHERE id = @id";
    public async Task<Kanji> GetKanjiAsync(int id)
    {
        SqliteCommand searchKanjiCmd = new SqliteCommand(KANJI_QUERY, sqliteConnection);
        searchKanjiCmd.Parameters.AddWithValue("@id", id);
        var result = await searchKanjiCmd.ExecuteReaderAsync();
        Kanji kanji = null;
        if (result.HasRows)
        {
            result.Read();
            kanji = new Kanji(result.GetInt32(0), result.SafeGetString(1), result.SafeGetString(2));
        }
        await searchKanjiCmd.DisposeAsync();
        return kanji;
    }

    public async Task<List<Kanji>> GetKanjisAsync(List<int> ids)
    {
        List<Kanji> kanjis = new List<Kanji>();
        foreach (var id in ids)
        {
            kanjis.Add(await GetKanjiAsync(id));
        }
        return kanjis;
    }

    const string SENSE_QUERY = "SELECT * FROM sense WHERE id = @id";
    public async Task<Sense> GetSenseAsync(int id)
    {
        SqliteCommand searchSenseCmd = new SqliteCommand(SENSE_QUERY, sqliteConnection);
        searchSenseCmd.Parameters.AddWithValue("@id", id);
        var result = await searchSenseCmd.ExecuteReaderAsync();
        Sense sense = null;
        if (result.HasRows)
        {
            result.Read();
            sense = new Sense(result.GetInt32(0), result.SafeGetString(1), result.SafeGetString(2),
                              result.SafeGetString(3), result.SafeGetString(4), result.SafeGetString(5),
                              "", "");
        }
        await searchSenseCmd.DisposeAsync();
        return sense;
    }

    public async Task<List<Sense>> GetSensesAsync(List<int> ids)
    {
        List<Sense> senses = new List<Sense>();
        foreach (var id in ids)
        {
            senses.Add(await GetSenseAsync(id));
        }
        return senses;
    }

    public async Task<List<SearchResult>> GetSearchResults(string keyword)
    {
        List<SearchResult> searchResults = new List<SearchResult>();
        List<Keyword> keywords = await SearchKeyword(keyword);
        foreach (var keywordEntry in keywords)
        {
            SearchResult searchResultEntry = new SearchResult(keywordEntry.Id, keywordEntry.Text, "", "", "");
            // query definition
            Definition definition = await GetDefinitionAsync(keywordEntry.DefEntryId);
            if (definition != null)
            {
                searchResultEntry.DefinitionSummary = definition.SummaryText.Replace(", ", ",").Replace(",", ", ");
            }
            searchResultEntry.Id = definition.Id;
            // check keyword type
            if (keywordEntry.KeywordType == KeywordType.English)
            {
                // set secondary field to kana
                var kana = await GetKanasAsync(definition.Kanas);
                searchResultEntry.SecondaryField = string.Join(", ", kana.Select(k => k.KanaStr));
            }
            else if (keywordEntry.KeywordType == KeywordType.Kanji)
            {
                var kana = await GetKanasAsync(definition.Kanas);
                searchResultEntry.SecondaryField = string.Join(", ", kana.Select(k => k.KanaStr));
                // set secondary field to kana
            }
            else if (keywordEntry.KeywordType == KeywordType.Reading)
            {
                var kanji = await GetKanjisAsync(definition.Kanjis);
                searchResultEntry.SecondaryField = string.Join(", ", kanji.Select(k => k.KanjiStr));
                // set secondary field to kanji
            }
            searchResults.Add(searchResultEntry);
        }
        return searchResults;
    }

    public async Task<DisplayDefinition> GetDisplayDefinitionAsync(int id)
    {
        SqliteCommand searchKeyWordCmd = new SqliteCommand(DEFINITION_QUERY, sqliteConnection);
        searchKeyWordCmd.Parameters.AddWithValue("@id", id);
        var result = await searchKeyWordCmd.ExecuteReaderAsync();
        Definition definition;
        if (result.HasRows)
        {
            result.Read();
            definition = new Definition(result.GetInt32(0), result.SafeGetString(1), result.SafeGetString(2), result.SafeGetString(3), result.SafeGetString(4));
            var kanas = await GetKanasAsync(definition.Kanas);
            var kanjis = await GetKanjisAsync(definition.Kanjis);
            var senses = await GetSensesAsync(definition.Senses);

            // create DisplayDefinition object
            DisplayDefinition displayDefinition = new DisplayDefinition(definition.Id, senses, kanjis, kanas);
            return displayDefinition;
        }
        await searchKeyWordCmd.DisposeAsync();
        return null;
    }
}
