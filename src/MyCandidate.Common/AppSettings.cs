using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyCandidate.Common;

public class AppSettings
{
    public const string APPLICATION_NAME = "MyCandidate";
    public const string JSON_FILE_NAME = "appsettings.json";
    public const string DB_FILE_NAME = "MyCandidate.db";
    public string DefaultLanguage { get; set; } = "en-US";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ThemeName DefaultTheme { get; set; } = ThemeName.Light;

    public string? Palette { get; set; } = null;
    public DatabaseSettings DatabaseSettings { get; set; } = new DatabaseSettings();

    public static string AppDataPath
    {
        get
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APPLICATION_NAME);
        }
    }

    public async Task SaveAsync()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JSON_FILE_NAME);
        if (File.Exists(filePath))
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            await using var fs = File.Open(filePath, FileMode.Create);
            await JsonSerializer.SerializeAsync(fs, this, options);
        }
    }
}

public class DatabaseSettings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SQLite;
    public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
}

public class ConnectionStrings
{
    public string SqlServer { get; set; } = string.Empty;
    public string PostgreSQL { get; set; } = string.Empty;
    public string SQLite { get; set; } = string.Empty;
}

public enum DatabaseType
{
    SqlServer,
    PostgreSQL,
    SQLite
}

public enum ThemeName
{
    Light,
    Dark
}
