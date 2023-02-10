using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogServer.ResourceViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogServer.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpGet("[action]/{id}")]
        public string Path(string id)
        {
            
            return Program.ProjectManager.GetPath(id) ?? "null";
        }
    }
}