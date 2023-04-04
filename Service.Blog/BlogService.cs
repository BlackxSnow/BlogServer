using Service;
using Utility;

namespace BlogServer;

public class BlogService : ServiceBase
{
    public readonly PostManager PostManager;
    
    public BlogService(string[] args) : base(args)
    {
        PostManager = new PostManager(Application);
        var postPath = Application.Configuration.GetValueThrow<string>("PostPath");
        PostManager.GeneratePostCache(postPath);
    }
}