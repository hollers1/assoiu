using Microsoft.AspNetCore.Mvc;

namespace homework3.Controllers;

/// <summary>
/// Контроллер главной страницы приложения.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Отображает главную страницу с навигационными карточками по разделам.
    /// </summary>
    /// <returns>Представление главной страницы.</returns>
    public IActionResult Index()
    {
        ViewData["Title"] = "Главная";
        return View();
    }
}
