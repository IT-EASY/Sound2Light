using CSCore.CoreAudioAPI;

using Sound2Light.Models.Audio;

using System.Collections.Generic;
using System.Diagnostics;

namespace Sound2Light.Services.Devices;

public class WasapiDeviceDiscovery : IWasapiDeviceDiscovery
{
    public List<AudioDevice> GetWasapiDevices()
    {
        var devices = new List<AudioDevice>();

        using var enumerator = new MMDeviceEnumerator();
        using var collection = enumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);

        foreach (var device in collection)
        {
            try
            {
                string name = device.FriendlyName?.Trim() ?? string.Empty;

                // Geräte mit unbrauchbarem Namen überspringen
                if (string.IsNullOrWhiteSpace(name) || name.StartsWith("{"))
                {
                    Debug.WriteLine($"[WASAPI] (skip) {name}");
                    continue;
                }

                using var client = AudioClient.FromMMDevice(device);
                var format = client.MixFormat;
                int sampleRate = format.SampleRate;
                int bitDepth = format.BitsPerSample;
                int channelCount = format.Channels;

                // Geräte ohne Input-Kanäle überspringen
                if (channelCount < 1)
                {
                    Debug.WriteLine($"[WASAPI] (skip) {name} - kein Input");
                    continue;
                }

                var audioDevice = new AudioDevice
                {
                    Name = name,
                    Type = AudioDeviceType.Wasapi,
                    SampleRate = sampleRate,
                    BitDepth = bitDepth,
                    ChannelCount = channelCount
                };

                Debug.WriteLine($"[WASAPI] {audioDevice.DisplayName} → {channelCount} Ch, {sampleRate} Hz, {bitDepth} Bit");
                devices.Add(audioDevice);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WASAPI] Fehler bei Gerät: {ex.Message}");
            }
        }

        return devices;
    }
}
