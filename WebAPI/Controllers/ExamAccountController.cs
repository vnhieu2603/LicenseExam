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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccountExam accountExam)
        {

            _context.AccountExams.Add(accountExam);
            await _context.SaveChangesAsync();

            return Ok(accountExam);
        }
    }
}
