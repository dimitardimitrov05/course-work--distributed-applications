using HotelSystem.MVC.Models;
using HotelSystem.MVC.Models.Hotel;
using HotelSystem.MVC.Models.Reservation;
using HotelSystem.MVC.Models.Room;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace HotelSystem.MVC.Controllers
{
    public class ReservationController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReservationController(IHttpClientFactory httpClientFactory)
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

            var response = await _httpClient.GetAsync($"api/reservation?page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<ReservationViewModel>>(json,
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

            var response = await _httpClient.GetAsync($"api/reservation/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var reservation = JsonSerializer.Deserialize<ReservationViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(reservation);
        }

        public async Task<IActionResult> Search(string? status, string? userId, int page = 1, int pageSize = 10)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/reservation/search?status={status}&userId={userId}&page={page}&pageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<ReservationViewModel>>(json,
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

            ViewBag.Hotels = result?.Data ?? new List<HotelViewModel>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReservationViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/reservation", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                return Content($"API грешка: {response.StatusCode} - {errorJson}");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/reservation/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var reservation = JsonSerializer.Deserialize<ReservationViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReservationViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/reservation/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Грешка при редактирането на резервация!");
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

            var response = await _httpClient.GetAsync($"api/reservation/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var reservation = JsonSerializer.Deserialize<ReservationViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return RedirectToAction("Login", "Account");

            SetAuthHeader();

            await _httpClient.DeleteAsync($"api/reservation/{id}");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetRoomsByHotel(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
                return Json(new List<RoomViewModel>());

            SetAuthHeader();

            var response = await _httpClient.GetAsync($"api/room/available?hotelId={hotelId}&checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}");
            var json = await response.Content.ReadAsStringAsync();
            var rooms = JsonSerializer.Deserialize<List<RoomViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Json(rooms);
        }
    }
}
