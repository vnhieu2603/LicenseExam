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
        //[HttpGet("odata/AnswerByQuestionID")]
        //[EnableQuery]
        //public async Task<IActionResult> GetAnswerByQuestionID([FromQuery] int question)
        //{
        //    var questions = await _context.Questions
        //                        .Where(q => q.Exams.Any(e => e.ExamId == examId))
        //                        .ToListAsync();

        //    if (questions == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(questions);
        //}
    }
}
