using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using Sound2Light.Config;
using Sound2Light.Settings;

namespace Sound2Light.Config
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly string _configFilePath;

        public AppSettings Settings { get; private set; }

        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };

        public AppConfigurationService(string configFilePath)
        {
            _configFilePath = configFilePath;
            Settings = new AppSettings();
        }

        public void LoadConfiguration()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    Debug.WriteLine($"[AppSettings] Datei nicht gefunden: {_configFilePath}");
                    return;
                }

                var json = File.ReadAllText(_configFilePath);
                Settings = JsonSerializer.Deserialize<AppSettings>(json, _serializerOptions) ?? new AppSettings();

                Debug.WriteLine("[AppSettings] Konfiguration erfolgreich geladen.");
            }
            catch (Exception ex)
            {
                Settings = new AppSettings();
                Debug.WriteLine($"[AppSettings] Fehler beim Laden der Konfiguration: {ex.Message}");
                Debug.WriteLine($"[AppSettings] StackTrace: {ex.StackTrace}");
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                var directory = Path.GetDirectoryName(_configFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var json = JsonSerializer.Serialize(Settings, _serializerOptions);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fehler beim Speichern der Konfiguration: {ex.Message}");
            }
        }
    }
}
