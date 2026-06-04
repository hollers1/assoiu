using homework2;

// База данных в папке приложения
var repo = new MuseumRepository("museums.db");
var report = new ReportBuilder(repo);

while (true)
{
    PrintMainMenu();
    var choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1": MenuMuseums(repo); break;
        case "2": MenuExhibits(repo); break;
        case "3": MenuReports(repo, report); break;
        case "0":
            Console.WriteLine("До свидания!");
            return;
        default:
            Console.WriteLine("Неверный выбор. Попробуйте снова.");
            break;
    }
}

// ══════════════════════════════════════════════════════════════
//  Главное меню
// ══════════════════════════════════════════════════════════════

static void PrintMainMenu()
{
    Console.WriteLine();
    Console.WriteLine("╔══════════════════════════════════╗");
    Console.WriteLine("║    Учёт музеев и экспонатов      ║");
    Console.WriteLine("╠══════════════════════════════════╣");
    Console.WriteLine("║  1. Управление музеями           ║");
    Console.WriteLine("║  2. Управление экспонатами       ║");
    Console.WriteLine("║  3. Отчёты                       ║");
    Console.WriteLine("║  0. Выход                        ║");
    Console.WriteLine("╚══════════════════════════════════╝");
    Console.Write("Выберите пункт: ");
}

// ══════════════════════════════════════════════════════════════
//  Подменю: Музеи
// ══════════════════════════════════════════════════════════════

static void MenuMuseums(MuseumRepository repo)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("── Музеи ──────────────────────────");
        Console.WriteLine("  1. Показать все музеи");
        Console.WriteLine("  2. Добавить музей");
        Console.WriteLine("  3. Изменить название музея");
        Console.WriteLine("  4. Удалить музей");
        Console.WriteLine("  0. Назад");
        Console.Write("Выберите: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                ShowMuseums(repo);
                break;

            case "2":
                Console.Write("Название нового музея: ");
                var name = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Название не может быть пустым.");
                    break;
                }
                var newId = repo.AddMuseum(name);
                Console.WriteLine($"Музей добавлен с ID={newId}.");
                break;

            case "3":
                ShowMuseums(repo);
                Console.Write("Введите ID музея для изменения: ");
                if (!int.TryParse(Console.ReadLine(), out int editId))
                {
                    Console.WriteLine("Некорректный ID.");
                    break;
                }
                Console.Write("Новое название: ");
                var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(newName))
                {
                    Console.WriteLine("Название не может быть пустым.");
                    break;
                }
                Console.WriteLine(repo.UpdateMuseum(editId, newName)
                    ? "Запись обновлена."
                    : "Музей с таким ID не найден.");
                break;

            case "4":
                ShowMuseums(repo);
                Console.Write("Введите ID музея для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int delId))
                {
                    Console.WriteLine("Некорректный ID.");
                    break;
                }
                Console.Write($"Удалить музей ID={delId} и все его экспонаты? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y") break;
                Console.WriteLine(repo.DeleteMuseum(delId)
                    ? "Музей и его экспонаты удалены."
                    : "Музей с таким ID не найден.");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Неверный выбор.");
                break;
        }
    }
}

static void ShowMuseums(MuseumRepository repo)
{
    var museums = repo.GetAllMuseums();
    if (museums.Count == 0)
    {
        Console.WriteLine("  (список музеев пуст)");
        return;
    }
    Console.WriteLine();
    foreach (var m in museums)
        Console.WriteLine($"  {m}");
    Console.WriteLine();
}

// ══════════════════════════════════════════════════════════════
//  Подменю: Экспонаты
// ══════════════════════════════════════════════════════════════

static void MenuExhibits(MuseumRepository repo)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("── Экспонаты ──────────────────────");
        Console.WriteLine("  1. Показать все экспонаты");
        Console.WriteLine("  2. Добавить экспонат");
        Console.WriteLine("  3. Изменить экспонат");
        Console.WriteLine("  4. Удалить экспонат");
        Console.WriteLine("  0. Назад");
        Console.Write("Выберите: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                ShowExhibits(repo);
                break;

            case "2":
                ShowMuseums(repo);
                if (repo.GetAllMuseums().Count == 0)
                {
                    Console.WriteLine("Сначала добавьте хотя бы один музей.");
                    break;
                }
                Console.Write("ID музея: ");
                if (!int.TryParse(Console.ReadLine(), out int mid) || repo.GetMuseumById(mid) == null)
                {
                    Console.WriteLine("Музей с таким ID не найден.");
                    break;
                }
                Console.Write("Название экспоната: ");
                var eName = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(eName))
                {
                    Console.WriteLine("Название не может быть пустым.");
                    break;
                }
                Console.Write("Год создания/поступления: ");
                if (!int.TryParse(Console.ReadLine(), out int year))
                {
                    Console.WriteLine("Некорректный год.");
                    break;
                }
                var eId = repo.AddExhibit(mid, eName, year);
                Console.WriteLine($"Экспонат добавлен с ID={eId}.");
                break;

            case "3":
                ShowExhibits(repo);
                Console.Write("ID экспоната для изменения: ");
                if (!int.TryParse(Console.ReadLine(), out int editEId))
                {
                    Console.WriteLine("Некорректный ID.");
                    break;
                }
                var existing = repo.GetExhibitById(editEId);
                if (existing == null)
                {
                    Console.WriteLine("Экспонат не найден.");
                    break;
                }
                ShowMuseums(repo);
                Console.Write($"Новый ID музея (текущий: {existing.MuseumId}): ");
                if (!int.TryParse(Console.ReadLine(), out int newMid) || repo.GetMuseumById(newMid) == null)
                {
                    Console.WriteLine("Музей с таким ID не найден.");
                    break;
                }
                Console.Write($"Новое название (текущее: {existing.ExhibitName}): ");
                var newEName = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(newEName))
                {
                    Console.WriteLine("Название не может быть пустым.");
                    break;
                }
                Console.Write($"Новый год (текущий: {existing.ExhibitYear}): ");
                if (!int.TryParse(Console.ReadLine(), out int newYear))
                {
                    Console.WriteLine("Некорректный год.");
                    break;
                }
                Console.WriteLine(repo.UpdateExhibit(editEId, newMid, newEName, newYear)
                    ? "Экспонат обновлён."
                    : "Ошибка обновления.");
                break;

            case "4":
                ShowExhibits(repo);
                Console.Write("ID экспоната для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int delEId))
                {
                    Console.WriteLine("Некорректный ID.");
                    break;
                }
                Console.WriteLine(repo.DeleteExhibit(delEId)
                    ? "Экспонат удалён."
                    : "Экспонат с таким ID не найден.");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Неверный выбор.");
                break;
        }
    }
}

static void ShowExhibits(MuseumRepository repo)
{
    var exhibits = repo.GetAllExhibits();
    if (exhibits.Count == 0)
    {
        Console.WriteLine("  (список экспонатов пуст)");
        return;
    }
    Console.WriteLine();
    foreach (var e in exhibits)
        Console.WriteLine($"  {e}");
    Console.WriteLine();
}

// ══════════════════════════════════════════════════════════════
//  Подменю: Отчёты (Fluent Interface)
// ══════════════════════════════════════════════════════════════

static void MenuReports(MuseumRepository repo, ReportBuilder report)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("── Отчёты ─────────────────────────");
        Console.WriteLine("  1. Все экспонаты (по ID)");
        Console.WriteLine("  2. Все экспонаты по году создания");
        Console.WriteLine("  3. Все экспонаты по названию");
        Console.WriteLine("  4. Экспонаты конкретного музея со статистикой");
        Console.WriteLine("  5. Экспонаты за диапазон лет со статистикой");
        Console.WriteLine("  0. Назад");
        Console.Write("Выберите: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                report.Reset()
                      .SetTitle("Все экспонаты")
                      .Print();
                break;

            case "2":
                report.Reset()
                      .SetTitle("Экспонаты — сортировка по году")
                      .SortByYear()
                      .Print();
                break;

            case "3":
                report.Reset()
                      .SetTitle("Экспонаты — сортировка по названию")
                      .SortByName()
                      .Print();
                break;

            case "4":
                ShowMuseums(repo);
                Console.Write("ID музея: ");
                if (!int.TryParse(Console.ReadLine(), out int mid))
                {
                    Console.WriteLine("Некорректный ID.");
                    break;
                }
                var museum = repo.GetMuseumById(mid);
                if (museum == null)
                {
                    Console.WriteLine("Музей не найден.");
                    break;
                }
                report.Reset()
                      .SetTitle($"Экспонаты музея «{museum.MuseumName}»")
                      .FilterByMuseum(mid)
                      .SortByYear()
                      .WithStatistics()
                      .Print();
                break;

            case "5":
                Console.Write("Год с: ");
                if (!int.TryParse(Console.ReadLine(), out int from))
                {
                    Console.WriteLine("Некорректный год.");
                    break;
                }
                Console.Write("Год по: ");
                if (!int.TryParse(Console.ReadLine(), out int to))
                {
                    Console.WriteLine("Некорректный год.");
                    break;
                }
                report.Reset()
                      .SetTitle($"Экспонаты за {from}–{to} гг.")
                      .FilterByYearRange(from, to)
                      .SortByYear()
                      .WithStatistics()
                      .Print();
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Неверный выбор.");
                break;
        }
    }
}
