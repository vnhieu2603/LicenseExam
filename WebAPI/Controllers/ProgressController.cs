using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();
        [HttpGet("getProgress")]
        public ActionResult Get(int userExamId)
        {
            var progress = _context.Progresses.Where(p => p.UserExamId == userExamId).ToList();
            return Ok(progress);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<Progress> progress)
        {
            foreach(var p in progress)
            {
                _context.Progresses.Add(p);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
