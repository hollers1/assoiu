namespace homework2;

/// <summary>
/// Основная таблица: Экспонат (сторона «много»).
/// exhibit(exhibit_id, museum_id, exhibit_name, exhibit_year)
/// </summary>
public class Exhibit
{
    public int ExhibitId { get; set; }
    public int MuseumId { get; set; }
    public string ExhibitName { get; set; } = string.Empty;
    public int ExhibitYear { get; set; }

    // Навигационное свойство (заполняется при JOIN-запросах)
    public string MuseumName { get; set; } = string.Empty;

    public override string ToString() =>
        $"[{ExhibitId}] {ExhibitName} ({ExhibitYear}) — музей: {MuseumName}";
}
