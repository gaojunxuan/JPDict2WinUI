using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPDict2.Core;
using NMeCab;
using NMeCab.Specialized;
using Windows.ApplicationModel;
using Windows.Storage;

namespace JPDict2.NLP;

public class Lemmatizer
{
    private static Lemmatizer _instance;
    private static readonly object _lock = new object();
    private MeCabUniDic21Tagger _tagger;

    private int _sensitivity = 0;
    public int Sensitivity
    {
        get => _sensitivity;
        set => _sensitivity = Math.Min(2, Math.Max(Sensitivity, 0));
    }

    private readonly int[][] _filteredPos = new int[][] {
        new int[] { },
        new int[] { 10, 31, 38, 39, 36, 47, 26, 59, 60, 34, 35, 67, 66, 61, 11, 12, 32, 33, 15, 25, 2, 58, 37, 68 },
        new int[] { 10, 31, 38, 39, 36, 47, 26, 59, 60, 34, 35, 67, 66, 58, 37, 68 },    
    };

    public static Lemmatizer Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new Lemmatizer();
                return _instance;
            }
        }
    }

    private Lemmatizer()
    {
        InitializeMeCab();
    }

    private void InitializeMeCab()
    {
        _tagger = MeCabUniDic21Tagger.Create(Path.Combine(ApplicationData.Current.LocalFolder.Path, "UniDic"));
    }

    public List<(string, string, string, string)> GetLemmatized(string sentence)
    {
        List<(string, string, string, string)> err = new List<(string, string, string, string)>();
        err.Add((sentence, "", "", ""));
        try
        {
            if (!string.IsNullOrWhiteSpace(sentence))
            {
                var nodes = _tagger.Parse(sentence);
                List<(string, string, string, string)> lemmatized = new List<(string, string, string, string)>();
                foreach (var node in nodes)
                {
                    if (Sensitivity == 0 || node.CharType > 0)
                    {
                        if (Sensitivity == 0 || (_filteredPos[Sensitivity].Contains(node.PosId)))
                        {
                            if (!string.IsNullOrWhiteSpace(node.Surface))
                            {
                                if (string.IsNullOrEmpty(node.Lemma))
                                    lemmatized.Add((node.Surface, node.Surface, "", ""));
                                else if (node.Surface.Any(char.IsPunctuation))
                                {
                                    lemmatized.Add((node.Surface, "", "", ""));
                                }
                                else if (node.Lemma.Contains("-") && node.Lemma.Length > 1)
                                {
                                    lemmatized.Add((node.Surface, node.Lemma.Split('-')[1], node.Pron, node.Pos1));
                                }
                                else
                                {
                                    string str = node.Lemma;
                                    if (str == "*")
                                        str = node.Surface;
                                    lemmatized.Add((node.Surface, str, node.Pron, node.Pos1));
                                }
                            }
                            //if (node.Surface == "死ね")
                            //    lemmatized.Add((node.Surface, "死ぬ", "シヌ", "動詞"));
                            //else if (node.Surface == "しね")
                            //    lemmatized.Add((node.Surface, "しぬ", "シヌ", "動詞"));
                            //else
                            //{
                            //    string str = features[features.Count() - 3];
                            //    if (str != "ない" && str != "する")
                            //    {
                            //        if (node.PosId == 33 && str == "いる")
                            //            lemmatized.Add((node.Surface, "いる", features[features.Count() - 2], features[0]));
                            //        else if (node.PosId == 37)
                            //            lemmatized.Add((node.Surface, str + "ない", features[features.Count() - 2] + "ナイ", features[0]));
                            //        else
                            //            lemmatized.Add((node.Surface, str, features[features.Count() - 2], features[0]));
                            //    }
                            //}
                            
                        }
                    }
                }
                return lemmatized.ToList();
            }
            return err;
        }
        catch
        {
            return err;
        }
    }
}
