using HotelSystem.MVC.Models;
using HotelSystem.MVC.Models.Account;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace HotelSystem.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HotelAPI");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Невалидно потребителско име или парола!");
                return View(model);
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var tokenObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
            var token = tokenObj.GetProperty("token").GetString();

            HttpContext.Session.SetString("JWTToken", token!);

            return RedirectToAction("Index", "Hotel");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Регистрацията е неуспешна!");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            return RedirectToAction("Login");
        }
    }
}
