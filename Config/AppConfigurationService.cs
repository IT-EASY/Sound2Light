using System;
using System.IO;
using System.Text.Json;

using Sound2Light.Settings;

namespace Sound2Light.Config
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly string _configFilePath;

        public AppSettings Settings { get; private set; }

        public AppConfigurationService(string configFilePath)
        {
            _configFilePath = configFilePath;
            Settings = new AppSettings(); // immer initialisiert
        }

        public void LoadConfiguration()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    Settings = new AppSettings();
                    return;
                }

                var json = File.ReadAllText(_configFilePath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);

                Settings = loaded ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Settings = new AppSettings();
                Console.Error.WriteLine($"Fehler beim Laden der Konfiguration: {ex.Message}");
                // TODO: LoggingService/EventLog einbinden
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

                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fehler beim Speichern der Konfiguration: {ex.Message}");
                // TODO: LoggingService/EventLog einbinden
            }
        }
    }
}
