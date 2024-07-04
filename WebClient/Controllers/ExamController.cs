using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class ExamController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _client;
        public ExamController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory;

        }
        public async Task<IActionResult> Index()
        {
            var client = _client.CreateClient();
            var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");
            if (tokenResponseJson != null)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
            }
            var response = await client.GetAsync("http://localhost:5275/odata/Exam");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var odataResponse = JsonConvert.DeserializeObject<ODataResponse<Exam>>(content);
            var examList = odataResponse.Value;
            ViewBag.examList = examList;

            return View(examList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _client.CreateClient();
            var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");
            if (tokenResponseJson != null)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
            }

            var response = await client.GetAsync($"http://localhost:5275/odata/Exam({id})");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var exam = JsonConvert.DeserializeObject<Exam>(content);
            ViewBag.exam = exam;

            // Assume exam duration is 30 minutes
            //int examDurationInMinutes = 30;
            //ViewBag.EndTime = DateTime.UtcNow.AddMinutes(examDurationInMinutes);

            int examDurationInMinutes = 1;
            DateTime endTime = DateTime.UtcNow.AddMinutes(examDurationInMinutes);

            ViewBag.EndTime = endTime;

            return View(exam);
        }
    }
}
