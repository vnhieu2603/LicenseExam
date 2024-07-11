using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Microsoft.AspNetCore.OData.Routing;
namespace WebAPI.Controllers
{   
    public class ExamController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        [Authorize]
        [EnableQuery]
        public ActionResult Get()
        {
            return Ok(_context.Exams.AsQueryable());

        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var exam = await _context.Exams
                            .Where(e => e.ExamId == key)
                            .FirstOrDefaultAsync();


            if (exam == null)
            {
                return NotFound();
            }

            

            return Ok(exam);
        }

        [Authorize]
        [HttpGet("odata/ExamByName")]
        [EnableQuery]
        public async Task<IActionResult> GetExamByName([FromQuery] string name)
        {
            var exam = await _context.Exams
                                .Where(e => e.Name == name)
                                .FirstOrDefaultAsync();

            if (exam == null)
            {
                return NotFound();
            }

            return Ok(exam);
        }
    }
}
