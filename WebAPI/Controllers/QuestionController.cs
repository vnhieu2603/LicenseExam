﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
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
    }
}