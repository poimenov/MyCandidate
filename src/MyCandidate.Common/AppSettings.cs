using System.Text.Json;

namespace MyCandidate.Common;

public class AppSettings
{
    public const string APPLICATION_NAME = "MyCandidate";
    public const string JSON_FILE_NAME = "appsettings.json";
    public string DefaultLanguage { get; set; }
    public string DefaultTheme { get; set; }

    public static string AppDataPath
    {
        get
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APPLICATION_NAME);
        }
    }

    public void Save()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JSON_FILE_NAME);
        if (File.Exists(filePath))
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, jsonString);
        }
    }
}
