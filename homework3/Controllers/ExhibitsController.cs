using homework3.Data;
using homework3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace homework3.Controllers;

/// <summary>
/// Контроллер для работы с основной таблицей экспонатов (detail-таблица).
/// Реализует CRUD-операции с выбором музея из выпадающего списка.
/// Запрещает ввод отрицательной оценочной стоимости (ValueK ≥ 0).
/// </summary>
public class ExhibitsController : Controller
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>
    /// Инициализирует контроллер фабрикой контекстов базы данных.
    /// </summary>
    /// <param name="factory">Фабрика для создания экземпляров <see cref="AppDbContext"/> через using.</param>
    public ExhibitsController(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Загружает список музеев в ViewBag.Museums для выпадающего списка формы.
    /// </summary>
    /// <param name="selectedId">Идентификатор выбранного музея (для Edit).</param>
    private async Task PopulateMuseumsDropDown(int? selectedId = null)
    {
        using var db = _factory.CreateDbContext();
        ViewBag.Museums = new SelectList(
            await db.Museums.OrderBy(m => m.MuseumName).ToListAsync(),
            "MuseumId", "MuseumName", selectedId);
    }

    // ── Index ────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает список всех экспонатов с названием музея (через Include).
    /// </summary>
    public async Task<IActionResult> Index()
    {
        using var db = _factory.CreateDbContext();
        var exhibits = await db.Exhibits
            .Include(e => e.Museum)
            .OrderBy(e => e.ExhibitName)
            .ToListAsync();
        ViewData["Title"] = "Экспонаты";
        return View(exhibits);
    }

    // ── Create ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает форму добавления нового экспоната с выпадающим списком музеев.
    /// </summary>
    public async Task<IActionResult> Create()
    {
        await PopulateMuseumsDropDown();
        ViewData["Title"] = "Добавить экспонат";
        return View();
    }

    /// <summary>
    /// Сохраняет новый экспонат в базе данных.
    /// Проверяет, что оценочная стоимость не отрицательна.
    /// </summary>
    /// <param name="exhibit">Данные нового экспоната из формы.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Exhibit exhibit)
    {
        if (!ModelState.IsValid)
        {
            await PopulateMuseumsDropDown(exhibit.MuseumId);
            ViewData["Title"] = "Добавить экспонат";
            return View(exhibit);
        }

        try
        {
            using var db = _factory.CreateDbContext();
            db.Exhibits.Add(exhibit);
            await db.SaveChangesAsync();
            TempData["Success"] = $"Экспонат «{exhibit.ExhibitName}» успешно добавлен.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await PopulateMuseumsDropDown(exhibit.MuseumId);
            ModelState.AddModelError(string.Empty, $"Ошибка при сохранении: {ex.Message}");
            ViewData["Title"] = "Добавить экспонат";
            return View(exhibit);
        }
    }

    // ── Edit ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает форму редактирования экспоната с выпадающим списком музеев.
    /// </summary>
    /// <param name="id">Идентификатор экспоната.</param>
    public async Task<IActionResult> Edit(int id)
    {
        using var db = _factory.CreateDbContext();
        var exhibit = await db.Exhibits.FindAsync(id);
        if (exhibit is null)
            return NotFound();

        await PopulateMuseumsDropDown(exhibit.MuseumId);
        ViewData["Title"] = "Редактировать экспонат";
        return View(exhibit);
    }

    /// <summary>
    /// Сохраняет изменения экспоната в базе данных.
    /// </summary>
    /// <param name="id">Идентификатор экспоната из маршрута.</param>
    /// <param name="exhibit">Обновлённые данные экспоната из формы.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Exhibit exhibit)
    {
        if (id != exhibit.ExhibitId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateMuseumsDropDown(exhibit.MuseumId);
            ViewData["Title"] = "Редактировать экспонат";
            return View(exhibit);
        }

        try
        {
            using var db = _factory.CreateDbContext();
            db.Exhibits.Update(exhibit);
            await db.SaveChangesAsync();
            TempData["Success"] = $"Экспонат «{exhibit.ExhibitName}» успешно обновлён.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            await PopulateMuseumsDropDown(exhibit.MuseumId);
            ModelState.AddModelError(string.Empty,
                "Запись была изменена другим пользователем. Обновите страницу и попробуйте снова.");
            ViewData["Title"] = "Редактировать экспонат";
            return View(exhibit);
        }
        catch (Exception ex)
        {
            await PopulateMuseumsDropDown(exhibit.MuseumId);
            ModelState.AddModelError(string.Empty, $"Ошибка при сохранении: {ex.Message}");
            ViewData["Title"] = "Редактировать экспонат";
            return View(exhibit);
        }
    }

    // ── Delete ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает страницу подтверждения удаления экспоната.
    /// </summary>
    /// <param name="id">Идентификатор экспоната.</param>
    public async Task<IActionResult> Delete(int id)
    {
        using var db = _factory.CreateDbContext();
        var exhibit = await db.Exhibits
            .Include(e => e.Museum)
            .FirstOrDefaultAsync(e => e.ExhibitId == id);

        if (exhibit is null)
            return NotFound();

        ViewData["Title"] = "Удалить экспонат";
        return View(exhibit);
    }

    /// <summary>
    /// Удаляет экспонат из базы данных после подтверждения пользователем.
    /// </summary>
    /// <param name="id">Идентификатор экспоната.</param>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            using var db = _factory.CreateDbContext();
            var exhibit = await db.Exhibits.FindAsync(id);
            if (exhibit is null)
                return NotFound();

            db.Exhibits.Remove(exhibit);
            await db.SaveChangesAsync();
            TempData["Success"] = $"Экспонат «{exhibit.ExhibitName}» успешно удалён.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Ошибка при удалении: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
