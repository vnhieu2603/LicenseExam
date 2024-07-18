using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ExamAccountController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        [HttpGet("getAllExamAccount")]
        public ActionResult Get()
        {
            return Ok(_context.AccountExams.ToList());
        }

        [HttpGet("getExamByExamAccount")]
        public ActionResult GetExamByExamAccount(int examAccountId)
        {
            var exams = _context.AccountExams.Include(ae => ae.Exam).Where(ae => ae.UserExamId == examAccountId).Select(ae => new AccountExamDTO
            {
                UserExamId = ae.UserExamId,
                ExamId = ae.ExamId,
                ExamName = ae.Exam.Name,
                Score = ae.Score,
                PassScore = ae.Exam.PassScore,
                Quantity = ae.Exam.Quantity
            }).FirstOrDefault();
            return Ok(exams);
        }

        public class AccountExamDTO
        {
            public AccountExamDTO()
            {
                Progresses = new HashSet<Progress>();
            }

            public int UserExamId { get; set; }
            public int? ExamId { get; set; }
            public string ExamName { get; set; }
            public double? Score { get; set; }
            public double? PassScore { get; set; }
            public int Quantity { get; set; }
            public virtual Exam? Exam { get; set; }
            public virtual Account? User { get; set; }
            public virtual ICollection<Progress> Progresses { get; set; }
        }

        [HttpGet("getExamByAccount")]
        public ActionResult GetExamByAccount([FromQuery] int userId)
        {
            var exams = _context.AccountExams.Include(ae => ae.Exam).Where(ae => ae.UserId == userId).Select(ae => new AccountExamDTO
            {
                UserExamId = ae.UserExamId,
                ExamId = ae.ExamId,
                ExamName = ae.Exam.Name,
                Score = ae.Score,
                PassScore = ae.Exam.PassScore,
                Quantity = ae.Exam.Quantity
            }).ToList();
            return Ok(exams);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccountExam accountExam)
        {

            _context.AccountExams.Add(accountExam);
            await _context.SaveChangesAsync();

            return Ok(accountExam);
        }

        [HttpPost("UpdateScore")]
        public async Task<IActionResult> UpdateScore([FromBody] AccountExam accountExam)
        {
            var exam = await _context.AccountExams.FindAsync(accountExam.UserExamId);
            if (exam == null)
            {
                return NotFound();
            }

            exam.Score = accountExam.Score;
            await _context.SaveChangesAsync();

            return Ok(exam);
        }
    }
}
