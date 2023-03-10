using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogServer.ResourceViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogServer.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        [HttpGet("[action]")]
        public int Count()
        {
            return Program.PostManager.PostsByDateAscending.Count;
        }

        [HttpGet("header/byindex")]
        public PostMetadata[] HeadersByIndexRange(int last, int count)
        {
            if (last < 0) last = Program.PostManager.PostsByDateAscending.Count - (last - 1);
            return Program.PostManager.PostsByDateAscending.Skip(last - count).Take(count)
                .Select(p => p.Metadata).Reverse().ToArray();
        }
        
        // GET: api/Blog/5
        [HttpGet("post/byid/{id}")]
        public PostView PostByID(string id)
        {
            return Program.PostManager.PostsByTitleID[id].GetPostView();
        }

        // POST: api/Blog
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Blog/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Blog/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
