using Sound2Light.Contracts.Services.Audio;
using Sound2Light.Models.Audio;
using Sound2Light.Config;

namespace Sound2Light.Services.Audio
{
    /// <summary>
    /// Liefert zur Laufzeit den jeweils richtigen Analyse-Buffer (WASAPI oder ASIO), je nach CurrentDevice.
    /// </summary>
    public class AudioBufferProvider
    {
        private readonly IAudioAnalysisBuffer _wasapiBuffer;
        // private readonly IAudioAnalysisBuffer _asioBuffer; // ASIO-Fall später ergänzen
        private readonly IAppConfigurationService _configService;

        public AudioBufferProvider(
            IAudioAnalysisBuffer wasapiBuffer,
            // IAudioAnalysisBuffer asioBuffer,
            IAppConfigurationService configService)
        {
            _wasapiBuffer = wasapiBuffer;
            // _asioBuffer = asioBuffer;
            _configService = configService;
        }

        /// <summary>
        /// Liefert den aktiven Buffer gemäß aktuellem DeviceType.
        /// </summary>
        public IAudioAnalysisBuffer GetActiveBuffer()
        {
            var current = _configService.Settings.CurrentDevice;
            if (current == null)
                throw new InvalidOperationException("Kein CurrentDevice konfiguriert!");

            return current.Type switch
            {
                AudioDeviceType.Wasapi => _wasapiBuffer,
                // AudioDeviceType.Asio => _asioBuffer, // Später ergänzen
                _ => throw new NotSupportedException("Aktueller DeviceType wird nicht unterstützt!")
            };
        }
    }
}
