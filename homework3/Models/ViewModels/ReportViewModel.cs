namespace homework3.Models.ViewModels;

/// <summary>
/// Модель представления для страницы аналитического отчёта.
/// Содержит данные для трёх разделов отчёта.
/// </summary>
public class ReportViewModel
{
    /// <summary>
    /// Раздел 1: Полный список экспонатов с названиями музеев,
    /// упорядоченный по названию экспоната.
    /// Формируется запросом LINQ: Include + OrderBy.
    /// </summary>
    public List<Exhibit> AllExhibits { get; set; } = new();

    /// <summary>
    /// Раздел 2: Количество экспонатов по каждому музею.
    /// Формируется запросом LINQ: GroupBy + Count.
    /// </summary>
    public List<MuseumCountDto> CountByMuseum { get; set; } = new();

    /// <summary>
    /// Раздел 3: Средняя оценочная стоимость экспонатов по музеям,
    /// отсортированная по убыванию средней стоимости.
    /// Формируется запросом LINQ: GroupBy + Average + OrderByDescending.
    /// </summary>
    public List<MuseumAvgValueDto> AvgValueByMuseum { get; set; } = new();
}

/// <summary>
/// DTO: количество экспонатов по одному музею (для раздела 2 отчёта).
/// </summary>
public class MuseumCountDto
{
    /// <summary>Название музея.</summary>
    public string MuseumName { get; set; } = string.Empty;

    /// <summary>Количество экспонатов в данном музее.</summary>
    public int Count { get; set; }
}

/// <summary>
/// DTO: средняя оценочная стоимость экспонатов по одному музею (для раздела 3 отчёта).
/// </summary>
public class MuseumAvgValueDto
{
    /// <summary>Название музея.</summary>
    public string MuseumName { get; set; } = string.Empty;

    /// <summary>Средняя оценочная стоимость экспонатов (тыс. руб.).</summary>
    public decimal AvgValueK { get; set; }
}
