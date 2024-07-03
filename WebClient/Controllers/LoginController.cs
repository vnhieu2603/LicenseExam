﻿using Microsoft.AspNetCore.Mvc;
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

        public class TokenResponse
        {
            public string Token { get; set; }
            public Account acc { get; set; }
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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("sai cmnr");

                    List<string> errors = new List<string>();
                    errors.Add("Email or password wrong");
                    ViewData["errors"] = errors;

                    return RedirectToAction("Privacy", "Home");


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }

        }
    }
}