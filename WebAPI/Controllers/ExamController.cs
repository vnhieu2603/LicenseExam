using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using WebAPI.Models;

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
    }
}
