using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5275/api/Login");
        private readonly HttpClient _client;

        public LoginController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            try
            {
                string data = JsonConvert.SerializeObject(account);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress, content).Result;
                Console.WriteLine(response);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseData);
                    HttpContext.Session.SetString("TokenResponse", JsonConvert.SerializeObject(tokenResponse));
                    if(tokenResponse.acc.RoleId == 2)
                    {
                        return RedirectToAction("Index", "Home");

                    } else
                    {
                        return RedirectToAction("Index", "Question");

                    }
                }
                else
                {
                    Console.WriteLine("sai cmnr");

                    List<string> errors = new List<string>();
                    errors.Add("Username or password wrong");
                    ViewData["errors"] = errors;

                    return View("Index");


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }

        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("TokenResponse");

            return RedirectToAction("Index", "Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Account account, string PasswordAgain)
        {
            Console.WriteLine("password: " + account.Password + " password again: " + PasswordAgain);
            if(!account.Password.Equals(PasswordAgain))
            {
                List<string> errors = new List<string>();
                errors.Add("Password và password nhập lại không trùng khớp ");
                ViewData["errors"] = errors;

                return View();
            }
            string data = JsonConvert.SerializeObject(account);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync("http://localhost:5275/api/Login/Register", content).Result;
            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dang ki thanh cong");
                return View("Index");
            }
            return View();
        }
    }
}
