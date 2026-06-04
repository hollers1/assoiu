using homework3.Data;
using homework3.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace homework3.Controllers;

/// <summary>
/// Контроллер аналитического отчёта по музейным экспонатам.
/// Формирует три раздела с использованием LINQ: Include, GroupBy, Count, Average, OrderBy.
/// </summary>
public class ReportController : Controller
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    /// <summary>
    /// Инициализирует контроллер фабрикой контекстов базы данных.
    /// </summary>
    /// <param name="factory">Фабрика для создания экземпляров <see cref="AppDbContext"/> через using.</param>
    public ReportController(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Формирует и отображает аналитический отчёт, содержащий три раздела:
    /// 1) Полный список экспонатов (Include + OrderBy);
    /// 2) Количество экспонатов по музеям (GroupBy + Count);
    /// 3) Средняя оценочная стоимость по музеям, убывание (GroupBy + Average + OrderByDescending).
    /// </summary>
    public async Task<IActionResult> Index()
    {
        using var db = _factory.CreateDbContext();

        // Раздел 1: Полный список экспонатов с названиями музеев
        // LINQ: Include (загрузка связанного музея) + OrderBy (по названию экспоната)
        var allExhibits = await db.Exhibits
            .Include(e => e.Museum)
            .OrderBy(e => e.ExhibitName)
            .ToListAsync();

        // Раздел 2: Количество экспонатов по музеям
        // LINQ: GroupBy + Count
        var countByMuseum = allExhibits
            .GroupBy(e => e.Museum!.MuseumName)
            .Select(g => new MuseumCountDto
            {
                MuseumName = g.Key,
                Count      = g.Count()
            })
            .OrderBy(x => x.MuseumName)
            .ToList();

        // Раздел 3: Среднее значение оценочной стоимости по музеям, сортировка по убыванию
        // LINQ: GroupBy + Average + OrderByDescending
        var avgValueByMuseum = allExhibits
            .GroupBy(e => e.Museum!.MuseumName)
            .Select(g => new MuseumAvgValueDto
            {
                MuseumName = g.Key,
                AvgValueK  = g.Average(e => e.ValueK)
            })
            .OrderByDescending(x => x.AvgValueK)
            .ToList();

        var model = new ReportViewModel
        {
            AllExhibits      = allExhibits,
            CountByMuseum    = countByMuseum,
            AvgValueByMuseum = avgValueByMuseum
        };

        ViewData["Title"] = "Аналитический отчёт";
        return View(model);
    }
}
