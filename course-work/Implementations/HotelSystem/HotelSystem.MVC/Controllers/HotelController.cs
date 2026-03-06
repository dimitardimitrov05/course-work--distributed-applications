using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using HotelSystem.MVC.Models;
using HotelSystem.MVC.Models.Hotel;

namespace HotelSystem.MVC.Controllers
{
    public class HotelController : Controller
    {
        private readonly HttpClient _httpClient;

        public HotelController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HotelAPI");
        }

        private void SetAuthHeader()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/hotel?page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<HotelViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalItems;

            return View(result.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/hotel/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var hotel = JsonSerializer.Deserialize<HotelViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(hotel);
        }

        public async Task<IActionResult> Search(string? name, string? city, int page = 1, int pageSize = 10)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/hotel/search?name={name}&city={city}&page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<HotelViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalItems;

            return View(result.Data);
        }

        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HotelViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/hotel", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Грешка при създаването на хотел!");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/hotel/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var hotel = JsonSerializer.Deserialize<HotelViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HotelViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/hotel/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Грешка при редактирането на хотел!");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/hotel/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var hotel = JsonSerializer.Deserialize<HotelViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            await _httpClient.DeleteAsync($"api/hotel/{id}");

            return RedirectToAction("Index");
        }
    }
}
