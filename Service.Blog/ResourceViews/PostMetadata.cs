using Microsoft.EntityFrameworkCore;

namespace BlogServer.ResourceViews;

public class PostMetadata
{
    public string Title { get; set; } = "Untitled";
    public string PreviewText { get; set; } = "";
    public DateTime PostTimeUTC { get; set; }
    
    public string ThumbnailPath { get; set; }
    public string UUID { get; set; }
    public string[] TitleID { get; set; }

    [NonSerialized] public string PostPath;
}

public class PostMetadataComparer : Comparer<BlogPost>
{
    public override int Compare(BlogPost? x, BlogPost? y)
    {
        return x?.Metadata.PostTimeUTC.CompareTo(y?.Metadata.PostTimeUTC) ?? -1;
    }
}