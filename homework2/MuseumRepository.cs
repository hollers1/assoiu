using Microsoft.Data.Sqlite;

namespace homework2;

/// <summary>
/// Репозиторий для работы с таблицами museum и exhibit в SQLite.
/// Все SQL-запросы используют параметризованные команды (защита от SQL-injection).
/// </summary>
public class MuseumRepository
{
    private readonly string _connectionString;

    public MuseumRepository(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        InitDatabase();
    }

    // ──────────────────────────── DDL ────────────────────────────

    private void InitDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS museum (
                museum_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                museum_name TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS exhibit (
                exhibit_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                museum_id    INTEGER NOT NULL REFERENCES museum(museum_id),
                exhibit_name TEXT    NOT NULL,
                exhibit_year INTEGER NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    // ──────────────────────────── Museum CRUD ────────────────────────────

    public List<Museum> GetAllMuseums()
    {
        var list = new List<Museum>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT museum_id, museum_name FROM museum ORDER BY museum_id;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Museum
            {
                MuseumId   = reader.GetInt32(0),
                MuseumName = reader.GetString(1)
            });
        }
        return list;
    }

    public Museum? GetMuseumById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT museum_id, museum_name FROM museum WHERE museum_id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Museum
            {
                MuseumId   = reader.GetInt32(0),
                MuseumName = reader.GetString(1)
            };
        }
        return null;
    }

    public int AddMuseum(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO museum (museum_name) VALUES ($name);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$name", name);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool UpdateMuseum(int id, string newName)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE museum SET museum_name = $name WHERE museum_id = $id;";
        cmd.Parameters.AddWithValue("$name", newName);
        cmd.Parameters.AddWithValue("$id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool DeleteMuseum(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        // Сначала удаляем связанные экспонаты
        using var delExhibits = conn.CreateCommand();
        delExhibits.CommandText = "DELETE FROM exhibit WHERE museum_id = $id;";
        delExhibits.Parameters.AddWithValue("$id", id);
        delExhibits.ExecuteNonQuery();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM museum WHERE museum_id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // ──────────────────────────── Exhibit CRUD ────────────────────────────

    public List<Exhibit> GetAllExhibits()
    {
        var list = new List<Exhibit>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT e.exhibit_id, e.museum_id, e.exhibit_name, e.exhibit_year, m.museum_name
            FROM exhibit e
            JOIN museum m ON e.museum_id = m.museum_id
            ORDER BY e.exhibit_id;
            """;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Exhibit
            {
                ExhibitId   = reader.GetInt32(0),
                MuseumId    = reader.GetInt32(1),
                ExhibitName = reader.GetString(2),
                ExhibitYear = reader.GetInt32(3),
                MuseumName  = reader.GetString(4)
            });
        }
        return list;
    }

    public List<Exhibit> GetExhibitsByMuseum(int museumId)
    {
        var list = new List<Exhibit>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT e.exhibit_id, e.museum_id, e.exhibit_name, e.exhibit_year, m.museum_name
            FROM exhibit e
            JOIN museum m ON e.museum_id = m.museum_id
            WHERE e.museum_id = $mid
            ORDER BY e.exhibit_id;
            """;
        cmd.Parameters.AddWithValue("$mid", museumId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Exhibit
            {
                ExhibitId   = reader.GetInt32(0),
                MuseumId    = reader.GetInt32(1),
                ExhibitName = reader.GetString(2),
                ExhibitYear = reader.GetInt32(3),
                MuseumName  = reader.GetString(4)
            });
        }
        return list;
    }

    public Exhibit? GetExhibitById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT e.exhibit_id, e.museum_id, e.exhibit_name, e.exhibit_year, m.museum_name
            FROM exhibit e
            JOIN museum m ON e.museum_id = m.museum_id
            WHERE e.exhibit_id = $id;
            """;
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Exhibit
            {
                ExhibitId   = reader.GetInt32(0),
                MuseumId    = reader.GetInt32(1),
                ExhibitName = reader.GetString(2),
                ExhibitYear = reader.GetInt32(3),
                MuseumName  = reader.GetString(4)
            };
        }
        return null;
    }

    public int AddExhibit(int museumId, string name, int year)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO exhibit (museum_id, exhibit_name, exhibit_year)
            VALUES ($mid, $name, $year);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$mid",  museumId);
        cmd.Parameters.AddWithValue("$name", name);
        cmd.Parameters.AddWithValue("$year", year);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool UpdateExhibit(int id, int museumId, string name, int year)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE exhibit
            SET museum_id = $mid, exhibit_name = $name, exhibit_year = $year
            WHERE exhibit_id = $id;
            """;
        cmd.Parameters.AddWithValue("$mid",  museumId);
        cmd.Parameters.AddWithValue("$name", name);
        cmd.Parameters.AddWithValue("$year", year);
        cmd.Parameters.AddWithValue("$id",   id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool DeleteExhibit(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM exhibit WHERE exhibit_id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}
