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

        [HttpPost]
        public async Task<IActionResult> UpdateQuestion(string questionid, string text, string answerId1, string answerId2, string answerId3, string answerId4,
            string answer1, string answer2, string answer3, string answer4, int CorrectAnswerIndex)
        {
            //Update question 
            Question updateQuestion = new Question
            {
                QuestionId = Int32.Parse(questionid),
                Text = text,
                //IsCritical = false,
                //TypeId = 1
            };
            var client = _client.CreateClient();
            var response = await client.PutAsJsonAsync("http://localhost:5275/api/Question", updateQuestion);
            int questionId = 0;
            if (response.IsSuccessStatusCode)
            {
                var createdQuestion = await response.Content.ReadAsStringAsync();
                var question = JsonConvert.DeserializeObject<Question>(createdQuestion);
                questionId = question.QuestionId;
                Console.WriteLine("question vua update: " + question.Text);
            }

            //Update answer for question
            List<Answer> answers = new List<Answer>();
            List<Answer> newAnswers = new List<Answer>();

            answers.Add(new Answer {AnswerId = Int32.Parse(answerId1), AnswerText = answer1, IsCorrect = CorrectAnswerIndex == 0, QuestionId = questionId });
            answers.Add(new Answer { AnswerId = Int32.Parse(answerId2), AnswerText = answer2, IsCorrect = CorrectAnswerIndex == 1, QuestionId = questionId });
            
            if (!string.IsNullOrEmpty(answerId3))
            {
                answers.Add(new Answer { AnswerId = Int32.Parse(answerId3), AnswerText = answer3, IsCorrect = CorrectAnswerIndex == 2, QuestionId = questionId });
            } else if(string.IsNullOrEmpty(answerId3) && !string.IsNullOrEmpty(answer3))
            {
                newAnswers.Add(new Answer { AnswerText = answer3, IsCorrect = CorrectAnswerIndex == 2, QuestionId = questionId });
            }
            if (!string.IsNullOrEmpty(answerId4))
            {
                answers.Add(new Answer { AnswerId = Int32.Parse(answerId4), AnswerText = answer4, IsCorrect = CorrectAnswerIndex == 3, QuestionId = questionId });
            }
            else if (string.IsNullOrEmpty(answerId4) && !string.IsNullOrEmpty(answer4))
            {
                newAnswers.Add(new Answer { AnswerText = answer4, IsCorrect = CorrectAnswerIndex == 3, QuestionId = questionId });
            }

            //update answer hien tai
            var client2 = _client.CreateClient();
            var response2 = await client2.PutAsJsonAsync("http://localhost:5275/api/Answer", answers);

            if (response2.IsSuccessStatusCode)
            {
                Console.WriteLine("update answer thanh cong");
            }
            else
            {
                Console.WriteLine("update answer that bai");

            }
            Console.WriteLine("new answers: " + newAnswers.Count());
            //add answer neu them moi
            if (newAnswers.Count() > 0)
            {
                var client3 = _client.CreateClient();
                var response3 = await client3.PostAsJsonAsync("http://localhost:5275/api/Answer", newAnswers);

                if (response3.IsSuccessStatusCode)
                {
                    Console.WriteLine("add answer thanh cong");
                }
                else
                {
                    Console.WriteLine("add answer that bai");

                }
            }

            Console.WriteLine("text: " + text);
            Console.WriteLine("answerId1: " + answerId1);
            Console.WriteLine("answerId2: " + answerId2);
            Console.WriteLine("answerId3: " + answerId3);
            Console.WriteLine("answerId4: " + answerId4);
            Console.WriteLine("answer 1: " + answer1);
            Console.WriteLine("answer 2: " + answer2);
            Console.WriteLine("answer 3: " + answer3);
            Console.WriteLine("answer 4: " + answer4);
            Console.WriteLine("CorrectAnswerIndex" + CorrectAnswerIndex);



            return RedirectToAction("Index", "Question");

        }

    }
}
