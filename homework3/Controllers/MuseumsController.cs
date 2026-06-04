using homework3.Data;
using homework3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace homework3.Controllers;

/// <summary>
/// Контроллер для работы со справочником музеев (master-таблица).
/// Реализует CRUD-операции: просмотр, добавление, редактирование, удаление.
/// Запрещает удаление музея, если у него есть связанные экспонаты.
/// </summary>
public class MuseumsController : Controller
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>
    /// Инициализирует контроллер фабрикой контекстов базы данных.
    /// </summary>
    /// <param name="factory">Фабрика для создания экземпляров <see cref="AppDbContext"/> через using.</param>
    public MuseumsController(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    // ── Index ────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает список всех музеев с количеством экспонатов в каждом.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        using var db = _factory.CreateDbContext();
        var museums = await db.Museums
            .Include(m => m.Exhibits)
            .OrderBy(m => m.MuseumName)
            .ToListAsync();
        ViewData["Title"] = "Музеи";
        return View(museums);
    }

    // ── Create ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает форму добавления нового музея.
    /// </summary>
    public IActionResult Create()
    {
        ViewData["Title"] = "Добавить музей";
        return View();
    }

    /// <summary>
    /// Сохраняет новый музей в базе данных.
    /// </summary>
    /// <param name="museum">Данные нового музея из формы.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Museum museum)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Добавить музей";
            return View(museum);
        }

        try
        {
            using var db = _factory.CreateDbContext();
            db.Museums.Add(museum);
            await db.SaveChangesAsync();
            TempData["Success"] = $"Музей «{museum.MuseumName}» успешно добавлен.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Ошибка при сохранении: {ex.Message}");
            ViewData["Title"] = "Добавить музей";
            return View(museum);
        }
    }

    // ── Edit ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает форму редактирования существующего музея.
    /// </summary>
    /// <param name="id">Идентификатор музея.</param>
    public async Task<IActionResult> Edit(int id)
    {
        using var db = _factory.CreateDbContext();
        var museum = await db.Museums.FindAsync(id);
        if (museum is null)
            return NotFound();

        ViewData["Title"] = "Редактировать музей";
        return View(museum);
    }

    /// <summary>
    /// Сохраняет изменения музея в базе данных.
    /// </summary>
    /// <param name="id">Идентификатор музея из маршрута (защита от подмены).</param>
    /// <param name="museum">Обновлённые данные музея из формы.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Museum museum)
    {
        if (id != museum.MuseumId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Редактировать музей";
            return View(museum);
        }

        try
        {
            using var db = _factory.CreateDbContext();
            db.Museums.Update(museum);
            await db.SaveChangesAsync();
            TempData["Success"] = $"Музей «{museum.MuseumName}» успешно обновлён.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty,
                "Запись была изменена другим пользователем. Обновите страницу и попробуйте снова.");
            ViewData["Title"] = "Редактировать музей";
            return View(museum);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Ошибка при сохранении: {ex.Message}");
            ViewData["Title"] = "Редактировать музей";
            return View(museum);
        }
    }

    // ── Delete ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Отображает страницу подтверждения удаления музея.
    /// Если у музея есть экспонаты — кнопка удаления не отображается.
    /// </summary>
    /// <param name="id">Идентификатор музея.</param>
    public async Task<IActionResult> Delete(int id)
    {
        using var db = _factory.CreateDbContext();
        var museum = await db.Museums
            .Include(m => m.Exhibits)
            .FirstOrDefaultAsync(m => m.MuseumId == id);

        if (museum is null)
            return NotFound();

        ViewData["Title"] = "Удалить музей";
        return View(museum);
    }

    /// <summary>
    /// Удаляет музей из базы данных после подтверждения пользователем.
    /// Отклоняет удаление, если у музея есть связанные экспонаты.
    /// </summary>
    /// <param name="id">Идентификатор музея.</param>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            using var db = _factory.CreateDbContext();
            var museum = await db.Museums
                .Include(m => m.Exhibits)
                .FirstOrDefaultAsync(m => m.MuseumId == id);

            if (museum is null)
                return NotFound();

            if (museum.Exhibits.Any())
            {
                TempData["Error"] =
                    $"Нельзя удалить музей «{museum.MuseumName}»: " +
                    $"у него есть {museum.Exhibits.Count} экспонат(ов). " +
                    "Сначала удалите или перенесите все экспонаты.";
                return RedirectToAction(nameof(Index));
            }

            db.Museums.Remove(museum);
            await db.SaveChangesAsync();
            TempData["Success"] = $"Музей «{museum.MuseumName}» успешно удалён.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Ошибка при удалении: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
