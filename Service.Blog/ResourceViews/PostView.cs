namespace BlogServer.ResourceViews;

public class PostView
{
    [NonSerialized] public BlogPost Post;

    public string Title => Post.Metadata.Title;
    public DateTime PostTimeUTC => Post.Metadata.PostTimeUTC;
    public string Thumbnail => Post.Metadata.ThumbnailPath;
    public string Text => Post.Text;
}

public static partial class ViewProviders
{
    public static PostView GetPostView(this BlogPost post)
    {
        return new PostView { Post = post };
    }
}