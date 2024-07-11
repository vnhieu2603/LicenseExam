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
                Console.WriteLine(tokenResponse.Token);

            }
            //Get exam
            var response = await client.GetAsync($"http://localhost:5275/odata/Exam({id})");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var exam = JsonConvert.DeserializeObject<Exam>(content);
            ViewBag.exam = exam;

            //Get question
            var client2 = _client.CreateClient();
            var response2 = await client2.GetAsync($"http://localhost:5275/odata/QuestionByExamID?examId={id}");

            if (response2.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            response2.EnsureSuccessStatusCode();
            Console.Write("vcl chay vao day roi ma");

            var content2 = await response2.Content.ReadAsStringAsync();
            var questions = JsonConvert.DeserializeObject<List<Question>>(content2);
            Console.WriteLine("answer of question 1: " + questions[2].Answers.ToList()[1].IsCorrect);
            ViewBag.questions = questions;

            int examDurationInMinutes = 1;
            DateTime endTime = DateTime.UtcNow.AddMinutes(examDurationInMinutes);

            ViewBag.EndTime = endTime;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(IFormCollection form)
        {
            var progresses = new List<Progress>();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("question_"))
                {
                    int questionId = int.Parse(key.Split('_')[1]);
                    int answerId = int.Parse(form[key]);

                    //var answer = await _context.Answers.FindAsync(answerId);
                    //var isCorrect = answer != null && answer.IsCorrect;

                    var progress = new Progress
                    {
                        QuestionId = questionId,
                        AnswerId = answerId,
                        //IsCorrect = isCorrect
                    };

                    progresses.Add(progress);
                }
            }

            foreach (var p in  progresses)
            {
                Console.WriteLine("Cau hoi: " + p.QuestionId + "Dap an lua chon: " + p.AnswerId);

            }

            //_context.Progresses.AddRange(progresses);
            //await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Hoặc hành động khác tùy theo logic của bạn
        }
    }
}
