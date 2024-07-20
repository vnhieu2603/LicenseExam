using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.Data.SqlClient;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ExamController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        //[Authorize]
        [HttpGet("getAllExam")]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Exams.ToList());

        }

        [EnableQuery]
        [HttpGet("getExamById")]

        public async Task<IActionResult> Get([FromQuery] int key)
        {
            var exam = await _context.Exams
                            .Where(e => e.ExamId == key)
                            .FirstOrDefaultAsync();
            Console.WriteLine("da lay dc exam id: " + key);

            if (exam == null)
            {
                return NotFound();
            }

            return Ok(exam);
        }

        [Authorize]
        [HttpGet("getExamByName")]
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

        public class ExamDto
        {
            public string Name { get; set; }
            public int Time { get; set; }
            public int Quantity { get; set; }
            public int PassScore { get; set; }
            public List<int> QuestionIds { get; set; }
        }

        [HttpPost("CreateExamWithQuestions")]
        public async Task<IActionResult> CreateExamWithQuestions([FromBody] ExamDto examDto)
        {
            if (examDto == null || examDto.QuestionIds == null || !examDto.QuestionIds.Any())
            {
                return BadRequest("Invalid exam data or no questions provided.");
            }

            // Insert Exam
            var sqlInsertExam = "INSERT INTO Exam (Name, Time, Quantity, PassScore) VALUES (@Name, @Time, @Quantity, @PassScore)";
            await _context.Database.ExecuteSqlRawAsync(
                sqlInsertExam,
                new SqlParameter("@Name", examDto.Name),
                new SqlParameter("@Time", examDto.Time),
                new SqlParameter("@Quantity", examDto.Quantity),
                new SqlParameter("@PassScore", examDto.PassScore)
            );
            var sqlGetLastExam = "SELECT TOP 1 * FROM Exam ORDER BY ExamId DESC";

            var lastExam = await _context.Exams
                .FromSqlRaw(sqlGetLastExam)
                .FirstOrDefaultAsync();
            // Insert into ExamQuestion
            foreach (var questionId in examDto.QuestionIds)
            {
                var sqlInsertExamQuestion = "INSERT INTO ExamQuestions (ExamId, QuestionId) VALUES (@ExamId, @QuestionId)";
                await _context.Database.ExecuteSqlRawAsync(sqlInsertExamQuestion,
                    new SqlParameter("@ExamId", lastExam.ExamId),
                    new SqlParameter("@QuestionId", questionId)
                );
            }

            return Ok(lastExam);
        }
    }
}
