using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
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

            int examDurationInMinutes = 1;
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
                examDurationInMinutes = exam.Time;
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

            AccountExam ae = new AccountExam
            {
                UserId = tokenResponse.acc.UserId,
                ExamId = examId
            };

            //Add account exam
            var client2 = _client.CreateClient();
            var response2 = await client2.PostAsJsonAsync("http://localhost:5275/api/ExamAccount", ae);
            var aeId = 0;
            if (response2.IsSuccessStatusCode)
            {
                var createdAccountExam = await response2.Content.ReadAsStringAsync();
                var accountExamObj = JsonConvert.DeserializeObject<AccountExam>(createdAccountExam);
                aeId = accountExamObj.UserExamId;
                Console.WriteLine("aeId = " + aeId);
            }

            var score = 0;
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
                    if(answer.IsCorrect == true)
                    {
                        score++;
                    }
                    var progress = new Progress
                    {
                        UserExamId = aeId,
                        QuestionId = questionId,
                        AnswerId = answerId,
                        IsCorrect = answer.IsCorrect
                    };

                    progresses.Add(progress);
                }
            }

            

            //Add progress 
            var client3 = _client.CreateClient();
            var response3 = await client3.PostAsJsonAsync("http://localhost:5275/api/Progress", progresses);

            if (response3.IsSuccessStatusCode)
            {
                Console.WriteLine("add progress thanh cong");
            }
            else
            {
                Console.WriteLine("add that bai");

            }

            //update exam score
            AccountExam ae2 = new AccountExam
            {
                UserExamId = aeId,
                Score = score,
            };
            var client4 = _client.CreateClient();
            var response4 = await client4.PostAsJsonAsync("http://localhost:5275/api/ExamAccount/UpdateScore", ae2);

            if (response4.IsSuccessStatusCode)
            {
                Console.WriteLine("update score thanh cong");
            }
            else
            {
                Console.WriteLine("update score that bai");

            }

            return RedirectToAction("HistoryExamDetail", new { userExamId = aeId, examId = examId });
        }

        public async Task<IActionResult> HistoryIndex()
        {

            var client = _client.CreateClient();
            var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");

                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
            
            var response = await client.GetAsync($"http://localhost:5275/api/ExamAccount/getExamByAccount?userId={tokenResponse.acc.UserId}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var examList = JsonConvert.DeserializeObject<List<AccountExamDTO>>(content);
            ViewBag.examList = examList;
            
            return View();
        }

        public async Task<IActionResult> HistoryExamDetail(int userExamId, int examId)
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
            var response = await client.GetAsync($"http://localhost:5275/api/Exam/getExamById?key={examId}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var exam = JsonConvert.DeserializeObject<Exam>(content);
                Console.WriteLine("Exam: " + exam);
                ViewBag.exam = exam;
            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }

            //Get question
            var client2 = _client.CreateClient();
            var response2 = await client2.GetAsync($"http://localhost:5275/api/Question/QuestionByExamID?examId={examId}");
            if (response2.IsSuccessStatusCode)
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

            //Get progress
            var client3 = _client.CreateClient();
            var response3 = await client3.GetAsync($"http://localhost:5275/api/Progress/getProgress?userExamId={userExamId}");
            if (response3.IsSuccessStatusCode)
            {
                var content3 = await response3.Content.ReadAsStringAsync();
                var progress = JsonConvert.DeserializeObject<List<Progress>>(content3);
                ViewBag.progress = progress;
            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");
            }

            //Get accountExam
            var client4 = _client.CreateClient();
            var response4 = await client4.GetAsync($"http://localhost:5275/api/ExamAccount/getExamByExamAccount?examAccountId={userExamId}");
            if (response4.IsSuccessStatusCode)
            {
                var content4 = await response4.Content.ReadAsStringAsync();
                var userExam = JsonConvert.DeserializeObject<AccountExamDTO>(content4);
                ViewBag.userExam = userExam;
            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");
            }
            return View();
        }

        public async Task<IActionResult> RandomExam()
        {
            //random name
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string randomString = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomExamName = "Exam" +randomString;

            //random quantity
            var client = _client.CreateClient();
            List<QuestionDTO> existquestions = new List<QuestionDTO>();
            var response = await client.GetAsync($"http://localhost:5275/api/Question/getAllQuestion");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                existquestions = JsonConvert.DeserializeObject<List<QuestionDTO>>(content);
            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }
            
            int randomQuantity = random.Next(3, existquestions.Count());
            int passScore = randomQuantity - 1;
            Console.WriteLine("Exam name: " + randomExamName);
            Console.WriteLine("Question count: " + existquestions.Count());
            Console.WriteLine("randomQuantity: " + randomQuantity);
            Console.WriteLine("passScore: " + passScore);

            //random question
            List<int> shuffledQuestionIds = existquestions
            .Select(q => q.QuestionId)
            .OrderBy(x => random.Next())
            .ToList();
            List<int> randomQuestionIds = shuffledQuestionIds.Take(randomQuantity).ToList();
            Console.WriteLine(randomQuestionIds.Count);

            //create random  exam 
            ExamDto newExam = new ExamDto
            {
                Name = randomExamName,
                Time = 5,
                Quantity = randomQuantity,
                PassScore = passScore,
                QuestionIds = randomQuestionIds
            };
            var client2 = _client.CreateClient();
            var response2 = await client2.PostAsJsonAsync("http://localhost:5275/api/Exam/CreateExamWithQuestions", newExam);
            var examId = 0;
            if (response2.IsSuccessStatusCode)
            {
                var createdExam = await response2.Content.ReadAsStringAsync();
                var ExamObj = JsonConvert.DeserializeObject<Exam>(createdExam);
                examId = ExamObj.ExamId;
                ViewBag.exam = ExamObj;

                Console.WriteLine("examId = " + examId);
            }

            var client3 = _client.CreateClient();
            var response3 = await client3.GetAsync($"http://localhost:5275/api/Question/QuestionByExamID?examId={examId}");


            if (response3.IsSuccessStatusCode)
            {
                var content3 = await response3.Content.ReadAsStringAsync();
                var questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(content3);
                ViewBag.questions = questions;

            }
            else
            {
                Console.WriteLine("call api fail");
                return RedirectToAction("Index", "Exam");

            }

            DateTime endTime = DateTime.UtcNow.AddMinutes(5);

            ViewBag.EndTime = endTime;
            return View("Details");
        }
    }
}
