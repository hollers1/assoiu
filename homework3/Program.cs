using homework3.Data;
using homework3.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрация MVC
builder.Services.AddControllersWithViews();

// Регистрация фабрики контекста: позволяет использовать "using var db = _factory.CreateDbContext()"
// в каждом методе контроллера, что соответствует требованию ТЗ о создании AppDbContext через using
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=museums.db"));

var app = builder.Build();

// ── Автоматическое создание БД и заполнение начальными данными ──────────────────
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = factory.CreateDbContext();          // создание контекста через using
    db.Database.EnsureCreated();                       // EnsureCreated — автосоздание схемы

    if (!db.Museums.Any())
    {
        // Минимум 4 записи в справочнике
        var museums = new List<Museum>
        {
            new() { MuseumName = "Государственный Эрмитаж" },
            new() { MuseumName = "Государственная Третьяковская галерея" },
            new() { MuseumName = "Русский музей" },
            new() { MuseumName = "Государственный музей изобразительных искусств им. А.С. Пушкина" },
        };
        db.Museums.AddRange(museums);
        db.SaveChanges();   // IDs присваиваются EF Core после SaveChanges

        // Минимум 12 записей в основной таблице (по 3 на музей)
        var exhibits = new List<Exhibit>
        {
            // Эрмитаж
            new() { MuseumId = museums[0].MuseumId, ExhibitName = "Мадонна Литта",              ExhibitYear = 1490, ValueK = 150_000m },
            new() { MuseumId = museums[0].MuseumId, ExhibitName = "Юноша в лавровом венке",      ExhibitYear = 1536, ValueK =  85_000m },
            new() { MuseumId = museums[0].MuseumId, ExhibitName = "Вольтер (скульптура Гудона)", ExhibitYear = 1781, ValueK =  45_000m },
            // Третьяковка
            new() { MuseumId = museums[1].MuseumId, ExhibitName = "Явление Христа народу",       ExhibitYear = 1857, ValueK = 120_000m },
            new() { MuseumId = museums[1].MuseumId, ExhibitName = "Утро в сосновом лесу",        ExhibitYear = 1889, ValueK =  98_000m },
            new() { MuseumId = museums[1].MuseumId, ExhibitName = "Девочка с персиками",         ExhibitYear = 1887, ValueK =  75_000m },
            // Русский музей
            new() { MuseumId = museums[2].MuseumId, ExhibitName = "Последний день Помпеи",       ExhibitYear = 1833, ValueK = 110_000m },
            new() { MuseumId = museums[2].MuseumId, ExhibitName = "Медный змий",                 ExhibitYear = 1841, ValueK =  65_000m },
            new() { MuseumId = museums[2].MuseumId, ExhibitName = "Волна",                        ExhibitYear = 1889, ValueK =  55_000m },
            // ГМИИ
            new() { MuseumId = museums[3].MuseumId, ExhibitName = "Сикстинская Мадонна (копия)", ExhibitYear = 1830, ValueK =  30_000m },
            new() { MuseumId = museums[3].MuseumId, ExhibitName = "Голубые танцовщицы",          ExhibitYear = 1897, ValueK = 200_000m },
            new() { MuseumId = museums[3].MuseumId, ExhibitName = "Красные виноградники",        ExhibitYear = 1888, ValueK = 180_000m },
        };
        db.Exhibits.AddRange(exhibits);
        db.SaveChanges();
    }
}

// ── HTTP pipeline ────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
