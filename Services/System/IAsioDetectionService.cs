using Sound2Light.Models.Audio;

namespace Sound2Light.Services.System
{
    public interface IAsioDetectionService
    {
        bool IsAsioAvailable();
        List<AsioDriverReference> GetAvailableDrivers();
    }
}
