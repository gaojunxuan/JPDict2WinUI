using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Models;
public class NHKNews
{
    public int Id
    {
        get; set;
    }
    public string Title
    {
        get; set;
    }
    public Uri Link
    {
        get; set;
    }
    public Uri OriginalLink
    {
        get; set;
    }
    public Uri ImgPath
    {
        get; set;
    }
    public Uri IconPath
    {
        get; set;
    }
    public Uri VideoPath
    {
        get; set;
    }
    public DateTime PubDate
    {
        get; set;
    }
    public bool IsEasy
    {
        get; set;
    }
}

public class RenderedNews
{
    public string Title
    {
        get; set;
    }
    public string Content
    {
        get; set;
    }
    public Uri Image
    {
        get; set;
    }
}
