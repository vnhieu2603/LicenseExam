﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ODataController
    {
        private readonly LicenseExamDBContext _context = new LicenseExamDBContext();

        [EnableQuery]
        [HttpGet("getAllAnswer")]
        public ActionResult Get()
        {
            return Ok(_context.Answers.ToList());
        }

        //[Authorize]
        [HttpGet("getAnswerById")]
        [EnableQuery]
        public async Task<IActionResult> GetAnswerByID([FromQuery] int answerId)
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

        [HttpGet("getAnswerByQuestionId")]
        [EnableQuery]
        public async Task<IActionResult> GetAnswerByQuestionID([FromQuery] int questionId)
        {
            var answers = await _context.Answers
                                .Where(a => a.QuestionId == questionId)
                                .ToListAsync();

            if (answers == null)
            {
                return NotFound();
            }

            return Ok(answers);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<Answer> answers)
        {
            foreach (var a in answers)
            {
                _context.Answers.Add(a);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] List<Answer> answers)
        {
            foreach (var a in answers)
            {
                var existingAnswer = await _context.Answers.FindAsync(a.AnswerId);
                existingAnswer.AnswerText = a.AnswerText;
                existingAnswer.IsCorrect = a.IsCorrect;

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

    }
}
