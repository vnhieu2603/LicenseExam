using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using WebClient.Models;
using static WebClient.Controllers.LoginController;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly IHttpClientFactory _client;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory;

        }

        //public IActionResult Index()
        //{
        //    var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");
        //    if (tokenResponseJson != null)
        //    {
        //        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);

        //        Console.WriteLine("Home: " + tokenResponse.Token + " " + tokenResponse.acc.UserId);
        //        return View();

        //    }
        //    else
        //    {
        //        return View("Unauthorized");
        //    }

        //}

        public async Task<IActionResult> Index()
        {
            var client = _client.CreateClient();
                var tokenResponseJson = HttpContext.Session.GetString("TokenResponse");
            if (tokenResponseJson != null)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
            }

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}