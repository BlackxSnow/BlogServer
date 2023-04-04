using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Utility;

namespace Projects.Service;

public class ProjectManager
{
    private ConcurrentDictionary<string, string> ProjectPathsById = new();
    private string ProjectDirectory = "";
    
    public string? GetPath(string id)
    {
        return ProjectPathsById.TryGetValue(id, out string? path) ? path : null;
    }

    private readonly Regex IDPattern = new Regex(@"[^\S\n\r]*\$project->path ?= ?""(.*)"";\r*$", 
        RegexOptions.Multiline | RegexOptions.Compiled);
    private DelayedFileWatcher _FileWatcher;
    private readonly ILogger _Logger;

    public ProjectManager(WebApplication application)
    {
        _Logger = application.CreateLogger<ProjectManager>();
    }
    
    private void RegenerateCache()
    {
        ProjectPathsById.Clear();
        if (!Directory.Exists(ProjectDirectory)) return;
        using (_Logger.BeginScope("Project cache regeneration"))
        {
            IEnumerable<string> projects =
                Directory.EnumerateFiles(ProjectDirectory, "*.php", SearchOption.AllDirectories);
            foreach (string path in projects)
            {
                _Logger.Log(LogLevel.Debug, $"Found project: {path}");
                string text = File.ReadAllText(path);
                Match idMatch = IDPattern.Match(text);
                bool wasAdded = ProjectPathsById.TryAdd(idMatch.Groups[1].Value, path);
                if (!wasAdded) _Logger.LogWarning($"{path} was not added to cache.");
                _Logger.Log(LogLevel.Debug,
                    $"Finished parsing {path}. Was added: {wasAdded}, ID: {idMatch.Groups[1].Value}");
            }

            _Logger.Log(LogLevel.Information, "Project cache regenerated");
        }
    }

    public void GenerateCache(string projectDirectory)
    {
        ProjectDirectory = projectDirectory;
        _FileWatcher = new DelayedFileWatcher(ProjectDirectory, "*.php", 
            NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite);
        _FileWatcher.Changed += (_, _) => RegenerateCache();
        _FileWatcher.Created += (_, _) => RegenerateCache();
        _FileWatcher.Deleted += (_, _) => RegenerateCache();
        _FileWatcher.Renamed += (_, _) => RegenerateCache();
        RegenerateCache();
        _FileWatcher.Start();
    }
}