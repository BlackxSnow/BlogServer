using Service;
using Utility;

namespace Projects.Service;

public class ProjectService : ServiceBase
{
    public readonly ProjectManager ProjectManager;
    
    public ProjectService(string[] args) : base(args)
    {
        ProjectManager = new ProjectManager(Application);
        var projectsPath = Application.Configuration.GetValueThrow<string>("ProjectPath");
        ProjectManager.GenerateCache(projectsPath);
    }
}