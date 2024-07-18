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
    [Route("api/[controller]")]
    [ApiController]

    public class QuestionController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        [EnableQuery]
        [HttpGet("getAllQuestion")]

        public async Task<IActionResult> Get()
        {
            var questions = await _context.Questions
                                .Include(q => q.Images)
                                .Include(q => q.Answers)
                                .Select(q => new
                                {
                                    q.QuestionId,
                                    q.Text,
                                    q.IsCritical,
                                    q.TypeId,
                                    q.Images,
                                    answers = _context.Answers.Where(a => a.QuestionId == q.QuestionId).Select(a => new AnswerDTO
                                    {
                                        AnswerId = a.AnswerId,
                                        AnswerText = a.AnswerText,
                                        IsCorrect = a.IsCorrect
                                    }).ToList()
                                })
                                .ToListAsync();

            if (questions == null)
            {
                return NotFound();
            }

            return Ok(questions);
        }

        [EnableQuery]
        [HttpGet("getQuestionById")]

        public async Task<IActionResult> GetQuestionById(int id)
        {
            var questions = await _context.Questions
                                .Include(q => q.Images)
                                .Include(q => q.Answers)
                                .Where(q => q.QuestionId == id)
                                .Select(q => new
                                {
                                    q.QuestionId,
                                    q.Text,
                                    q.IsCritical,
                                    q.TypeId,
                                    q.Images,
                                    answers = _context.Answers.Where(a => a.QuestionId == q.QuestionId).Select(a => new AnswerDTO
                                    {
                                        AnswerId = a.AnswerId,
                                        AnswerText = a.AnswerText,
                                        IsCorrect = a.IsCorrect
                                    }).ToList()
                                })
                                .FirstOrDefaultAsync();

            if (questions == null)
            {
                return NotFound();
            }

            return Ok(questions);
        }


        public class AnswerDTO
        {
            public AnswerDTO()
            {
                Progresses = new HashSet<Progress>();
            }

            public int AnswerId { get; set; }
            public string AnswerText { get; set; } = null!;
            public bool IsCorrect { get; set; }
            public virtual ICollection<Progress> Progresses { get; set; }
        }

        //[Authorize]
        [HttpGet("QuestionByExamID")]
        [EnableQuery]
        public async Task<IActionResult> GetQuestionByExamID([FromQuery] int examId)
        {
            var questions = await _context.Questions
                                .Include(q => q.Images)
                                .Include(q => q.Answers)
                                .Where(q => q.Exams.Any(e => e.ExamId == examId))
                                .Select(q => new
                                {
                                    q.QuestionId,
                                    q.Text,
                                    q.IsCritical,
                                    q.TypeId,
                                    q.Images,
                                    answers = _context.Answers.Where(a => a.QuestionId == q.QuestionId).Select(a => new AnswerDTO
                                    {
                                        AnswerId = a.AnswerId,
                                        AnswerText = a.AnswerText,
                                        IsCorrect = a.IsCorrect
                                    }).ToList()
                                })
                                .ToListAsync();

            if (questions == null)
            {
                return NotFound();
            }

            return Ok(questions);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Question question)
        {

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(question);
        }
    }
}
