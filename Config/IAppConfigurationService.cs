using Sound2Light.Settings;

namespace Sound2Light.Config
{
    public interface IAppConfigurationService
    {
        void LoadConfiguration();
        void SaveConfiguration();

        AppSettings Settings { get; }
    }
}
