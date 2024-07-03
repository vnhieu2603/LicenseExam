using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers
{
    public class Login : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
