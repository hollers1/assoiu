using System.ComponentModel.DataAnnotations;

namespace homework3.Models;

/// <summary>
/// Справочная таблица: Музей (сторона «один» в связи один-ко-многим).
/// Соответствует таблице museum(museum_id, museum_name).
/// </summary>
public class Museum
{
    /// <summary>Уникальный идентификатор музея (первичный ключ).</summary>
    public int MuseumId { get; set; }

    /// <summary>Полное название музея.</summary>
    [Required(ErrorMessage = "Название музея обязательно.")]
    [StringLength(200, MinimumLength = 2,
        ErrorMessage = "Название должно содержать от 2 до 200 символов.")]
    [Display(Name = "Название музея")]
    public string MuseumName { get; set; } = string.Empty;

    /// <summary>
    /// Навигационное свойство-коллекция: экспонаты данного музея.
    /// Заполняется при использовании Include в запросах.
    /// </summary>
    public ICollection<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
}
