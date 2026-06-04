using Microsoft.EntityFrameworkCore;
using homework3.Models;

namespace homework3.Data;

/// <summary>
/// Контекст базы данных приложения (Code First, SQLite).
/// Предоставляет доступ к таблицам Museums и Exhibits через Entity Framework Core.
/// Создаётся через <see cref="IDbContextFactory{TContext}"/> и конструкцию using.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AppDbContext"/>.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста базы данных.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Набор данных: Музеи (справочная таблица, сторона «один»).
    /// </summary>
    public DbSet<Museum> Museums => Set<Museum>();

    /// <summary>
    /// Набор данных: Экспонаты (основная таблица, сторона «много»).
    /// </summary>
    public DbSet<Exhibit> Exhibits => Set<Exhibit>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Запрет каскадного удаления: нельзя удалить музей при наличии связанных экспонатов
        modelBuilder.Entity<Exhibit>()
            .HasOne(e => e.Museum)
            .WithMany(m => m.Exhibits)
            .HasForeignKey(e => e.MuseumId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
