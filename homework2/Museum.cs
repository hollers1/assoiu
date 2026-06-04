namespace homework2;

/// <summary>
/// Справочная таблица: Музей (сторона «один»).
/// museum(museum_id, museum_name)
/// </summary>
public class Museum
{
    public int MuseumId { get; set; }
    public string MuseumName { get; set; } = string.Empty;

    public override string ToString() =>
        $"[{MuseumId}] {MuseumName}";
}
