using System.ComponentModel.DataAnnotations;

namespace homework3.Models;

/// <summary>
/// Основная таблица: Экспонат (сторона «много» в связи один-ко-многим).
/// Соответствует таблице exhibit(exhibit_id, museum_id, exhibit_name, exhibit_year, value_k).
/// </summary>
public class Exhibit
{
    /// <summary>Уникальный идентификатор экспоната (первичный ключ).</summary>
    public int ExhibitId { get; set; }

    /// <summary>Внешний ключ: идентификатор музея, которому принадлежит экспонат.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Необходимо выбрать музей.")]
    [Display(Name = "Музей")]
    public int MuseumId { get; set; }

    /// <summary>Название экспоната.</summary>
    [Required(ErrorMessage = "Название экспоната обязательно.")]
    [StringLength(300, MinimumLength = 2,
        ErrorMessage = "Название должно содержать от 2 до 300 символов.")]
    [Display(Name = "Название экспоната")]
    public string ExhibitName { get; set; } = string.Empty;

    /// <summary>Год создания экспоната.</summary>
    [Range(0, 2100, ErrorMessage = "Год должен быть в диапазоне от 0 до 2100.")]
    [Display(Name = "Год создания")]
    public int ExhibitYear { get; set; }

    /// <summary>
    /// Оценочная стоимость экспоната в тысячах рублей (числовой показатель value_k).
    /// Значение не может быть отрицательным.
    /// </summary>
    [Range(typeof(decimal), "0", "9999999999",
        ErrorMessage = "Оценочная стоимость не может быть отрицательной.")]
    [Display(Name = "Оценочная стоимость (тыс. руб.)")]
    public decimal ValueK { get; set; }

    /// <summary>
    /// Навигационное свойство-ссылка: музей, которому принадлежит экспонат.
    /// Заполняется при использовании Include в запросах.
    /// </summary>
    public Museum? Museum { get; set; }
}
