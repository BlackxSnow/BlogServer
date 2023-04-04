using Microsoft.AspNetCore.Mvc;

namespace Projects.Service
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpGet("[action]/{id}")]
        public string Path(string id)
        {
            
            return Program.ServiceInstance.ProjectManager.GetPath(id) ?? "null";
        }
    }
}