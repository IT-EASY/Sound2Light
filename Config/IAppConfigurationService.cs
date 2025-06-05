using Sound2Light.Config;
using Sound2Light.Settings;

namespace Sound2Light.Config
{
    public interface IAppConfigurationService
    {
        AppSettings Settings { get; }
        void LoadConfiguration();
        void SaveConfiguration();
    }
}
