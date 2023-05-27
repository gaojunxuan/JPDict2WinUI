using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPDict2.Core.Extensions;
using JPDict2.Core.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace JPDict2.Core;
public class KanjiDictQueryEngine
{
    private static KanjiDictQueryEngine _instance;
    private static readonly object _lock = new object();

    public static KanjiDictQueryEngine Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new KanjiDictQueryEngine();
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

    private KanjiDictQueryEngine()
    {
    }

    public async void InitializeDatabase()
    {
        bool isDatabaseExisting = true;
        if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("kanjidict.db") == null)
        {
            isDatabaseExisting = false;
        }
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path,
                                     "kanjidict.db");
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
        var dbFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/kanjidict.db"));
        await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, "kanjidict.db", NameCollisionOption.ReplaceExisting);
    }

    const string KEYWORD_EXACT_QUERY = "SELECT * FROM kanjientry WHERE kanji = @keyword";
    public async Task<KanjiEntry> GetKanjiEntryAsync(string keyword)
    {

        var command = sqliteConnection.CreateCommand();
        command.CommandText = KEYWORD_EXACT_QUERY;
        command.Parameters.AddWithValue("@keyword", keyword);
        var exactResult = await command.ExecuteReaderAsync();
        KanjiEntry kanjiEntry = null;
        if (exactResult.HasRows)
        {
            await exactResult.ReadAsync();
            kanjiEntry = new KanjiEntry()
            {
                Id = exactResult.GetInt32(0),
                Kanji = exactResult.SafeGetString(1),
                StrokeCount = exactResult.GetInt32(2),
                RadicalIds = exactResult.SafeGetString(3).Split(',').Select(int.Parse).ToList(),
                Grade = exactResult.GetInt32(4),
                JLPTLevel = exactResult.GetInt32(5),
                Frequency = exactResult.GetInt32(6),
                OnReading = exactResult.SafeGetString(7),
                KunReading = exactResult.SafeGetString(8),
                Meanings = exactResult.SafeGetString(9)
            };
            return kanjiEntry;
        }
        return null;
    }

    public async Task<List<KanjiEntry>> GetKanjiEntriesAsync(List<string> keywords)
    {
        List<KanjiEntry> entries = new List<KanjiEntry>();
        foreach (var kw in keywords)
        {
            var entry = await GetKanjiEntryAsync(kw);
            if (entry != null)
            {
                entries.Add(entry);
            }
        }
        return entries;
    }

    static string[] Radicals =
    {
        "一", "丨", "丶", "丿", "乙 / 乚 (right) / 乛", "亅", "二", "亠", 
        "人 / 亻 (left)", "儿", "入", "八 / 丷 (top)", "冂 (enclosure)", "冖 (top)", "冫 (left)", 
        "几", "凵 (enclosure)", "刀 / 刂 (right) / ⺈(top)", "力", "勹 (enclosure)", 
        "匕", "匚 (enclosure)", "匸 (enclosure)", "十", "卜 (right)", "㔾 / 卩 (right)", 
        "厂", "厶", "又", "口", "囗 (enclosure)", "土", "士", "夂", "夊", "夕", "大", 
        "女", "子", "宀 (top)", "寸", "⺌ (top) / ⺍ (top) / 小", "尢", "尸 (enclosure)", 
        "屮", "山", "巛 (top) / 川", "工", "己", "巾", "干", "幺 (left) / 么", "广 (enclosure)", 
        "廴 (enclosure)", "廾", "弋 (enclosure)", "弓", "彐 (bottom) / 彑", "彡 (right)", 
        "彳 (left)", "心", "戈", "户", "手 / 扌 (left) / 龵 (enclosure)", "支", "攴", "文", 
        "斗", "斤", "方", "无", "日", "曰", "月", "木", "欠", "止", "歹", "殳", "母 / 毋", "比", 
        "毛", "氏", "气", "水 / 氵 (left)", "火 / 灬 (bottom)", "爪 / 爫 (top)", "父", "爻", 
        "爿 / 丬 (left)", "片", "牙", "牛 / 牜(left) / ⺧ (top)", "犬", "玄", "玉 / 王", "瓜", 
        "瓦", "甘", "生", "用", "田", "疋 / ⺪(top)", "疒 (enclosure)", "癶", "白", "皮", "皿", 
        "目", "矛", "矢", "石", "示 / 礻 (left)", "禸", "禾", "穴", "立", "竹 / ⺮ (top)", "米", 
        "糸 / 糹 (left)", "缶", "网", "羊 / ⺷ (top)", "羽", "老/耂", "而", "耒", "耳", 
        "聿 / ⺻ (top)", "肉 / ⺼ (left)", "臣", "自", "至", "臼", "舌", "舛", "舟", "艮", "色", 
        "⺿ (top) / 艸", "虍 (enclosure)", "虫", "血", "行", "⻂ (left) / 衣", "西", "見", "角", 
        "言", "谷", "豆", "豕", "豸", "貝", "赤", "走", "足", "身", "車", "辛", "辰", "⻌", 
        "⻏ (right) / 邑", "酉", "釆", "里", "金", "長", "門 (enclosure)", "⻖ (left) / 阜", "隶", 
        "隹", "雨", "青", "非", "面", "革", "韋", "韭", "音", "頁", "風", "飛", "食", "首", "香", 
        "馬", "骨", "高", "髟", "鬥 (enclosure)", "鬯", "鬲", "鬼", "魚", "鳥", "鹵", "鹿", "麥", 
        "麻", "黃", "黍", "黑", "黹", "黽", "鼎", "鼓", "鼠", "鼻", "斉", "齒", "龍", "龜", "龠"
    };

    public static string ConvertRadicalIdToRadicalLiteral(int radicalId)
    {
        return Radicals[radicalId - 1];
    }

    const string KANJIGUIDE_KEYWORD_QUERY = "SELECT * FROM kanjiwritingguide WHERE kanji = @keyword";
    public async Task<List<KanjiGuideStep>> GetKanjiWritingGuideAsync(string keyword)
    {
        SqliteCommand command = new SqliteCommand(KANJIGUIDE_KEYWORD_QUERY, sqliteConnection);
        command.Parameters.AddWithValue("@keyword", keyword);
        var result = await command.ExecuteReaderAsync();

        List<KanjiGuideStep> kanjiGuide = new List<KanjiGuideStep>();

        while (await result.ReadAsync())
        {
            var kanjiGuideStep = new KanjiGuideStep
            {
                Kanji = result.SafeGetString(0),
                Order = result.GetInt32(2),
                SVGPath = result.SafeGetString(1)
            };
            kanjiGuide.Add(kanjiGuideStep);
        }
        return kanjiGuide.OrderBy(k => k.Order).ToList();
    }
}
