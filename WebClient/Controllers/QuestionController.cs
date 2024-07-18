using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebClient.DTO;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;

        private readonly IHttpClientFactory _client;
        public QuestionController(ILogger<QuestionController> logger, IHttpClientFactory httpClientFactory)
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
                Console.WriteLine(tokenResponse.Token);
            }
            //Get questions
            var response = await client.GetAsync($"http://localhost:5275/api/Question/getAllQuestion");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(content);

                ViewBag.questions = questions;
            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _client.CreateClient();
            var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");
            if (tokenResponseJson != null)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                Console.WriteLine(tokenResponse.Token);
            }
            //Get questions
            var response = await client.GetAsync($"http://localhost:5275/api/Question/getQuestionById?id={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var questions = JsonConvert.DeserializeObject<QuestionDTO>(content);
                ViewBag.questions = questions;
            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }
            return View();
        }

        public async Task<IActionResult> AddQuestion()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddQuestion(string text, string answer1, string answer2, string answer3, string answer4,int CorrectAnswerIndex)
        {
            //Add question 
            Question newQuestion = new Question
            {
                Text = text,
                IsCritical = false,
                TypeId = 1
            };
            var client = _client.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5275/api/Question", newQuestion);
            int questionId = 0;
            if (response.IsSuccessStatusCode)
            {
                var createdQuestion = await response.Content.ReadAsStringAsync();
                var question = JsonConvert.DeserializeObject<Question>(createdQuestion);
                questionId = question.QuestionId;
                Console.WriteLine("question vua tao: " + question.Text);
            }
            Console.WriteLine("correct answer index: " + CorrectAnswerIndex);
            //Add answer for question
            List<Answer> answers = new List<Answer>();
            answers.Add(new Answer { AnswerText = answer1, IsCorrect = CorrectAnswerIndex == 0, QuestionId = questionId });
            answers.Add(new Answer { AnswerText = answer2, IsCorrect = CorrectAnswerIndex == 1, QuestionId = questionId });
            if (!string.IsNullOrEmpty(answer3))
            {
                answers.Add(new Answer { AnswerText = answer3, IsCorrect = CorrectAnswerIndex == 2, QuestionId = questionId });
            }
            if (!string.IsNullOrEmpty(answer4))
            {
                answers.Add(new Answer { AnswerText = answer4, IsCorrect = CorrectAnswerIndex == 3, QuestionId = questionId });
            }
            var client2 = _client.CreateClient();
            var response2 = await client2.PostAsJsonAsync("http://localhost:5275/api/Answer", answers);

            if (response2.IsSuccessStatusCode)
            {
                Console.WriteLine("add answer thanh cong");
            }
            else
            {
                Console.WriteLine("add answer that bai");

            }

            return RedirectToAction("Index", "Question");

        }

    }
}
