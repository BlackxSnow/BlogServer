using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using BlogServer.ResourceViews;
using BlogServer.Utility;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace BlogServer;

public class PostManager
{
    private readonly ILogger _Logger = Logging.CreateLogger<PostManager>();
    
    public SortedSet<BlogPost> PostsByDateAscending { get; private set; } = new(new PostMetadataComparer());
    public Dictionary<string, BlogPost> PostsByFileName { get; private set; } = new();
    public Dictionary<string, BlogPost> PostsByTitleID { get; private set; } = new();

    private DelayedFileWatcher _DelayedFileWatcher;
    // private FileSystemWatcher _Watcher;


    private void AddPost(BlogPost post, string fileName)
    {
        PostsByDateAscending.Add(post);
        PostsByFileName.Add(fileName, post);
        PostsByTitleID.Add(post.Metadata.TitleID[0], post);
    }
    private void RemovePost(string fileName, BlogPost post)
    {
        PostsByDateAscending.Remove(post);
        PostsByFileName.Remove(fileName);
        PostsByTitleID.Remove(post.Metadata.TitleID[0]);
    }
    
    private bool RemovePost(string fileName, [NotNullWhen(true)] out BlogPost? post)
    {
        if (!PostsByFileName.TryGetValue(fileName, out post)) return false;
        RemovePost(fileName, post);
        return true;
    }
    
    private void StartFileWatcher(string path)
    {
        _DelayedFileWatcher = new DelayedFileWatcher(path, "*.post", NotifyFilters.FileName | NotifyFilters.LastWrite);
        _DelayedFileWatcher.Changed += OnMetaChanged;
        _DelayedFileWatcher.Renamed += OnMetaRenamed;
        _DelayedFileWatcher.Deleted += OnMetaDeleted;
        _DelayedFileWatcher.Start();
        // _Watcher = new FileSystemWatcher(path, "*.post");
        // _Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
        // _Watcher.IncludeSubdirectories = true;
        // _Watcher
    }

    private readonly Regex _MetaPattern = new Regex(@"^(.+?):[^\S\n\r]*(.*)$(?=[\s\S]*---)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private const int PreviewTextLength = 297;
    private readonly Regex _PreviewPattern = new Regex($@"[\s\S]{{0,{PreviewTextLength}}}(?= )", RegexOptions.Compiled);
    private async Task LoadPost(BlogPost toFill, string path)
    {
        try
        {
            string postData = await File.ReadAllTextAsync(path);
            Dictionary<string, string> metadataPairs = _MetaPattern.Matches(postData)
                .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(metadataPairs["Timezone"]);
            int bodyStartIndex = postData.IndexOf("---", StringComparison.Ordinal) + 3;
            toFill.Metadata = new PostMetadata
            {
                PostPath = path,
                PostTimeUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(metadataPairs["PostTime"], CultureInfo.GetCultureInfo("en-AU")), timeZone),
                ThumbnailPath = metadataPairs["Thumbnail"],
                Title = metadataPairs["Title"],
                TitleID = metadataPairs["IDs"].Split(',', StringSplitOptions.TrimEntries),
                PreviewText = _PreviewPattern.Match(postData, bodyStartIndex).Value + "..."
                // PreviewText = postData.Substring(bodyStartIndex, PreviewTextLength)
            };
            toFill.Text = postData[bodyStartIndex..];
        }
        catch (Exception e)
        {
            _Logger.LogError(e, $"Error populating metadata from {path}", null!);
        }
        _Logger.LogDebug($"Loaded post at {path}.");
    }

    private async void OnMetaChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType.HasFlag(WatcherChangeTypes.Renamed) ||
            e.ChangeType.HasFlag(WatcherChangeTypes.Deleted)) return;
        if (!RemovePost(Path.GetFileName(e.FullPath), out BlogPost? post)) post = new BlogPost();
        await LoadPost(post, e.FullPath);
        AddPost(post, Path.GetFileName(e.FullPath));
    }
    
    private async void OnMetaRenamed(object sender, RenamedEventArgs e)
    {
        if (!RemovePost(Path.GetFileName(e.OldFullPath), out BlogPost? post)) return;
        await LoadPost(post, e.FullPath);
        AddPost(post, Path.GetFileName(e.FullPath));
    }
    
    private void OnMetaDeleted(object sender, FileSystemEventArgs e)
    {
        if (!RemovePost(Path.GetFileName(e.FullPath), out _)) return;
        _Logger.LogDebug($"Removed post {Path.GetFileName(e.FullPath)}");
    }
    
    public async void GeneratePostCache(string directory)
    {
        var generatedPostSet = new SortedSet<BlogPost>(new PostMetadataComparer());
        IEnumerable<string> posts = Directory.EnumerateFiles(directory, "*.post", SearchOption.AllDirectories);
        foreach (string postPath in posts)
        {
            var post = new BlogPost();
            try
            {
                await LoadPost(post, postPath);
            }
            catch (Exception e)
            {
                _Logger.LogError(e, null, null!);
                continue;
            }
            generatedPostSet.Add(post);
            PostsByFileName.Add(Path.GetFileName(postPath), post);
            foreach (string id in post.Metadata.TitleID)
            {
                PostsByTitleID.Add(id, post);
            }
        }

        PostsByDateAscending = generatedPostSet;
        StartFileWatcher(directory);
    }
}