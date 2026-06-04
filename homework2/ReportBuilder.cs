namespace homework2;

/// <summary>
/// Построитель отчётов с паттерном Fluent Interface.
/// Каждый метод возвращает this, что позволяет цепочку вызовов:
///   new ReportBuilder(repo)
///       .SetTitle("...")
///       .FilterByMuseum(id)
///       .SortByYear()
///       .Print();
/// Примечание: LINQ не используется (будет изучен позже).
/// </summary>
public class ReportBuilder
{
    private readonly MuseumRepository _repo;
    private string _title = "Отчёт";
    private int? _museumFilter = null;
    private bool _sortByYear = false;
    private bool _sortByName = false;
    private int? _yearFrom = null;
    private int? _yearTo = null;
    private bool _showStatistics = false;

    public ReportBuilder(MuseumRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Установить заголовок отчёта.</summary>
    public ReportBuilder SetTitle(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>Фильтровать экспонаты по музею.</summary>
    public ReportBuilder FilterByMuseum(int museumId)
    {
        _museumFilter = museumId;
        return this;
    }

    /// <summary>Сортировать по году создания.</summary>
    public ReportBuilder SortByYear()
    {
        _sortByYear = true;
        _sortByName = false;
        return this;
    }

    /// <summary>Сортировать по названию экспоната.</summary>
    public ReportBuilder SortByName()
    {
        _sortByName = true;
        _sortByYear = false;
        return this;
    }

    /// <summary>Фильтровать экспонаты по диапазону лет.</summary>
    public ReportBuilder FilterByYearRange(int from, int to)
    {
        _yearFrom = from;
        _yearTo   = to;
        return this;
    }

    /// <summary>Включить в отчёт статистику.</summary>
    public ReportBuilder WithStatistics()
    {
        _showStatistics = true;
        return this;
    }

    /// <summary>Напечатать отчёт в консоль.</summary>
    public ReportBuilder Print()
    {
        // 1. Получаем исходный список из репозитория
        List<Exhibit> source = _museumFilter.HasValue
            ? _repo.GetExhibitsByMuseum(_museumFilter.Value)
            : _repo.GetAllExhibits();

        // 2. Фильтрация по диапазону лет (без LINQ)
        var filtered = new List<Exhibit>();
        foreach (var ex in source)
        {
            bool ok = true;
            if (_yearFrom.HasValue && ex.ExhibitYear < _yearFrom.Value) ok = false;
            if (_yearTo.HasValue   && ex.ExhibitYear > _yearTo.Value)   ok = false;
            if (ok) filtered.Add(ex);
        }

        // 3. Сортировка методом вставки (без LINQ)
        if (_sortByYear)
        {
            InsertionSort(filtered, (a, b) => a.ExhibitYear.CompareTo(b.ExhibitYear));
        }
        else if (_sortByName)
        {
            InsertionSort(filtered, (a, b) => string.Compare(a.ExhibitName, b.ExhibitName,
                                                              StringComparison.CurrentCulture));
        }

        // 4. Вывод заголовка
        Console.WriteLine();
        Console.WriteLine(new string('═', 60));
        Console.WriteLine($"  {_title}");
        Console.WriteLine(new string('═', 60));

        if (filtered.Count == 0)
        {
            Console.WriteLine("  (нет данных)");
        }
        else
        {
            foreach (var ex in filtered)
                Console.WriteLine($"  {ex}");
        }

        // 5. Статистика (без LINQ)
        if (_showStatistics && filtered.Count > 0)
        {
            int minYear = filtered[0].ExhibitYear;
            int maxYear = filtered[0].ExhibitYear;
            long sumYear = 0;

            foreach (var ex in filtered)
            {
                if (ex.ExhibitYear < minYear) minYear = ex.ExhibitYear;
                if (ex.ExhibitYear > maxYear) maxYear = ex.ExhibitYear;
                sumYear += ex.ExhibitYear;
            }

            double avgYear = (double)sumYear / filtered.Count;

            Console.WriteLine(new string('─', 60));
            Console.WriteLine($"  Итого экспонатов : {filtered.Count}");
            Console.WriteLine($"  Самый ранний год : {minYear}");
            Console.WriteLine($"  Самый поздний год: {maxYear}");
            Console.WriteLine($"  Средний год      : {avgYear:F0}");
        }

        Console.WriteLine(new string('═', 60));
        Console.WriteLine();
        return this;
    }

    /// <summary>Сбросить все фильтры и настройки.</summary>
    public ReportBuilder Reset()
    {
        _title          = "Отчёт";
        _museumFilter   = null;
        _sortByYear     = false;
        _sortByName     = false;
        _yearFrom       = null;
        _yearTo         = null;
        _showStatistics = false;
        return this;
    }

    // ──────────────────────────── Вспомогательный метод ────────────────────────────

    /// <summary>Сортировка вставками (без LINQ).</summary>
    private static void InsertionSort(List<Exhibit> list, Comparison<Exhibit> compare)
    {
        for (int i = 1; i < list.Count; i++)
        {
            var key = list[i];
            int j = i - 1;
            while (j >= 0 && compare(list[j], key) > 0)
            {
                list[j + 1] = list[j];
                j--;
            }
            list[j + 1] = key;
        }
    }
}
