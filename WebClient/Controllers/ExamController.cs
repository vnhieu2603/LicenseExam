using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebClient.DTO;
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
            var response = await client.GetAsync("http://localhost:5275/api/Exam/getAllExam");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var examList = JsonConvert.DeserializeObject<List<Exam>>(content);
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
            var response = await client.GetAsync($"http://localhost:5275/api/Exam/getExamById?key={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var exam = JsonConvert.DeserializeObject<Exam>(content);
                Console.WriteLine("Exam: " + exam);
                ViewBag.exam = exam;
            } else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }


            //Get question
            var client2 = _client.CreateClient();
            var response2 = await client2.GetAsync($"http://localhost:5275/api/Question/QuestionByExamID?examId={id}");

            if (response2.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            
            
            if(response2.IsSuccessStatusCode)
            {
                var content2 = await response2.Content.ReadAsStringAsync();
                var questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(content2);
                ViewBag.questions = questions;

            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }

            int examDurationInMinutes = 1;
            DateTime endTime = DateTime.UtcNow.AddMinutes(examDurationInMinutes);

            ViewBag.EndTime = endTime;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(IFormCollection form, int examId)
        {
            var progresses = new List<Progress>();
            var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);

            Console.WriteLine("Lay duoc examId: " + examId + " va userId: " + tokenResponse.acc.UserId);

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("question_"))
                {
                    int questionId = int.Parse(key.Split('_')[1]);
                    int answerId = int.Parse(form[key]);

                    var client = _client.CreateClient();
                    var response = await client.GetAsync($"http://localhost:5275/api/Answer/getAnswerById?answerId={answerId}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Index", "Login");
                    }
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var answer = JsonConvert.DeserializeObject<Answer>(content);

                    var progress = new Progress
                    {
                        QuestionId = questionId,
                        AnswerId = answerId,
                        IsCorrect = answer.IsCorrect
                    };

                    progresses.Add(progress);
                }
            }

            foreach (var p in  progresses)
            {
                Console.WriteLine("Cau hoi: " + p.QuestionId + " Dap an lua chon: " + p.AnswerId + " Ket qua: " + p.IsCorrect);

            }

            //_context.Progresses.AddRange(progresses);
            //await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Hoặc hành động khác tùy theo logic của bạn
        }
    }
}
