using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AnswerController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        [EnableQuery]
        public ActionResult Get()
        {
            return Ok(_context.Answers.AsQueryable());
        }

        //[Authorize]
        [HttpGet("odata/AnswerById")]
        [EnableQuery]
        public async Task<IActionResult> GetAnswerByQuestionID([FromQuery] int answerId)
        {
            var answers = await _context.Answers
                                .Where(a => a.AnswerId == answerId)
                                .FirstOrDefaultAsync();

            if (answers == null)
            {
                return NotFound();
            }

            return Ok(answers);
        }
    }
}
