// Sound2Light/Services/Audio/AudioServiceFactory.cs
using Sound2Light.Models;
using Sound2Light.Settings;

namespace Sound2Light.Services.Audio
{
    public class AudioServiceFactory
    {
        public IAudioService Create(AudioApiType type) => type switch
        {
            AudioApiType.ASIO => new ASIOAudioService(),
            AudioApiType.WASAPI => new WASAPIAudioService(),
            _ => throw new NotSupportedException()
        };
    }
}