using HotelSystem.MVC.Models;
using HotelSystem.MVC.Models.Hotel;
using HotelSystem.MVC.Models.Room;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
namespace HotelSystem.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient _httpClient;

        public RoomController(IHttpClientFactory httpClientFactory)
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

            var response = await _httpClient.GetAsync($"api/room?page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<RoomViewModel>>(json,
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

            var response = await _httpClient.GetAsync($"api/room/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var room = JsonSerializer.Deserialize<RoomViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(room);
        }

        public async Task<IActionResult> Search(int hotelId, string? roomType, bool? isAvailable, int page = 1, int pageSize = 10)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/room/search?hotelId={hotelId}&roomType={roomType}&isAvailable={isAvailable}&page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<RoomViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalItems;

            return View(result.Data);
        }

        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync("api/hotel?page=1&pageSize=100");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<HotelViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Hotels = result.Data;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/room", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Грешка при създаването на стая!");
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

            var response = await _httpClient.GetAsync($"api/room/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var room = JsonSerializer.Deserialize<RoomViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RoomViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/room/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Грешка при редактирането на стая!");
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

            var response = await _httpClient.GetAsync($"api/room/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var room = JsonSerializer.Deserialize<RoomViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            await _httpClient.DeleteAsync($"api/room/{id}");

            return RedirectToAction("Index");
        }
    }
}
