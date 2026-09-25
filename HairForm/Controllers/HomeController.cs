using HairForm.Classes;
using HairForm.Database;
using HairForm.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace HairForm.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormService _formService;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly ApplicationDbContext _context;
        public HomeController(FormService formService, IStringLocalizer<HomeController> localizer, ApplicationDbContext context)
        {
            _formService = formService;
            _localizer = localizer;
            _context = context;
        }

        [HttpPost]
        public IActionResult Index(Order model, Dictionary<AccessoryType, int> accessories)
        {
            var result = _formService.FromCreate(model, accessories);
            if (result is ErrorContainer error)
            {
                // Передаём ошибку через TempData и делаем Redirect на GET Index
                TempData["ErrorMessage"] = error.ErrorText;
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Заказ успешно создан!";
            return RedirectToAction("Index");
        }
        private async Task<User> GetAdminAsync()
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Role == Role.Admin);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var admin = await GetAdminAsync();
            ViewBag.IsAcceptingOrders = admin?.IsAcceptingOrders ?? true;

            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            return View(new Order());
        }

        [HttpGet]
        public async Task<IActionResult> GetBookedDates()
        {
            var bookedDates = await _formService.GetBookedDates();
            return Json(bookedDates);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                var cookie = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    cookie,
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
