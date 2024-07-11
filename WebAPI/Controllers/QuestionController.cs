using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebAPI.Models;

namespace WebAPI.Controllers
{

    public class QuestionController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        [EnableQuery]
        public ActionResult Get()
        {
            return Ok(_context.Questions.AsQueryable());
        }

        //[Authorize]
        [HttpGet("odata/QuestionByExamID")]
        [EnableQuery]
        public async Task<IActionResult> GetQuestionByExamID([FromQuery] int examId)
        {
            var questions = await _context.Questions
                                .Include(q => q.Answers)
                                .Include(q => q.Images)
                                .Where(q => q.Exams.Any(e => e.ExamId == examId))
                                .ToListAsync();

            if (questions == null)
            {
                return NotFound();
            }

            return Ok(questions);
        }
    }
}
