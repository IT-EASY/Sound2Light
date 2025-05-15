// Sound2Light/Services/Audio/IAudioService.cs  
using Sound2Light.Models;
using Sound2Light.Common;
using Sound2Light.Settings;

namespace Sound2Light.Services.Audio
{
    public interface IAudioService
    {
        void Initialize(AudioDeviceInfo device, AudioSettings settings);
        void Start();
        void Stop();
        void Dispose();

        event EventHandler<AudioDataEventArgs> DataAvailable;
        event EventHandler<ErrorEventArgs>? ErrorOccurred;
    }
}