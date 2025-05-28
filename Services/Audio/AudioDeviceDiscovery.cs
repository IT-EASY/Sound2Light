using System;
using System.Diagnostics;
using System.Collections.Generic;

using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.Asio;

using Sound2Light.Models.Audio;
using Sound2Light.Settings;

namespace Sound2Light.Services.Audio
{
    public static class AudioDeviceDiscovery
    {


        // Dummy-WaveProvider zur Übergabe der gewünschten SampleRate
        private class DummyWaveProvider : IWaveProvider
        {
            public WaveFormat WaveFormat { get; }

            public DummyWaveProvider(int sampleRate, int channels = 1)
            {
                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }
        }

        public static List<DeviceInfo> GetAsioDevices(AppSettings settings)
        {
            var result = new List<DeviceInfo>();
            var driverNames = AsioOut.GetDriverNames();
            Debug.WriteLine($"Gefundene ASIO-Treiber: {string.Join(", ", driverNames)}");

            foreach (var driverName in driverNames)
            {
                // 🛑 Blacklist-Filter
                if (settings.IgnoredAsioDrivers.Contains(driverName))
                {
                    Debug.WriteLine($"ASIO ignoriert (Blacklist): {driverName}");
                    continue;
                }

                try
                {
                    using var asio = new AsioOut(driverName);

                    if (asio.DriverInputChannelCount < 2)
                    {
                        Debug.WriteLine($"ASIO ignoriert (zu wenig Kanäle): {driverName}");
                        continue;
                    }

                    int sampleRate = 0;
                    bool initSuccess = false;

                    try
                    {
                        var dummyProvider = new DummyWaveProvider(48000);
                        asio.InitRecordAndPlayback(dummyProvider, 1, 48000);
                        sampleRate = dummyProvider.WaveFormat.SampleRate;
                        Debug.WriteLine($"{driverName} → Init mit 48 kHz erfolgreich");
                        initSuccess = true;
                    }
                    catch
                    {
                        try
                        {
                            var dummyProvider = new DummyWaveProvider(44100);
                            asio.InitRecordAndPlayback(dummyProvider, 1, 44100);
                            sampleRate = dummyProvider.WaveFormat.SampleRate;
                            Debug.WriteLine($"{driverName} → Init mit 44.1 kHz erfolgreich (Fallback)");
                            initSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"ASIO Init fehlgeschlagen ({driverName}): {ex.Message}");
                        }
                    }

                    if (!initSuccess || sampleRate < 44100)
                    {
                        Debug.WriteLine($"ASIO verworfen (Init fehlgeschlagen oder SampleRate < 44100): {driverName}");
                        continue;
                    }

                    int bufferSize = asio.FramesPerBuffer;

                    var device = new DeviceInfo
                    {
                        Type = AudioDeviceType.ASIO,
                        Name = driverName,
                        IsAvailable = true,
                        PreferredInputChannels = asio.DriverInputChannelCount,
                        PreferredSampleRate = sampleRate,
                        PreferredBufferSize = bufferSize,
                        PreferredBitDepth = 24 // Annahme, da nicht direkt ermittelbar
                    };

                    result.Add(device);
                    Debug.WriteLine($"ASIO akzeptiert: {driverName} → {sampleRate} Hz, Buffer {bufferSize} Frames");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ASIO-Treiber '{driverName}' übersprungen (außerhalb): {ex.Message}");
                }
            }

            return result;
        }

        public static List<DeviceInfo> GetWasapiDevices()
        {
            var result = new List<DeviceInfo>();
            var enumerator = new MMDeviceEnumerator();

            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var device in devices)
            {
                try
                {
                    var props = device.AudioClient.MixFormat;

                    if (props == null || props.Channels < 2 || props.SampleRate < 44100)
                        continue;

                    if (props.BitsPerSample != 0 && props.BitsPerSample < 16)
                        continue;

                    int? bufferSizeFrames = null;

                    try
                    {
                        // Versuche Initialisierung im Exclusive Mode mit 100ms Buffer
                        long bufferDuration100ns = 1000000; // 100 ms
                        device.AudioClient.Initialize(
                            AudioClientShareMode.Exclusive,
                            AudioClientStreamFlags.None,
                            bufferDuration100ns,
                            bufferDuration100ns,
                            props,
                            Guid.Empty);

                        bufferSizeFrames = device.AudioClient.BufferSize;

                        Debug.WriteLine($"WASAPI (Exclusive): {device.FriendlyName} → BufferSize = {bufferSizeFrames} Frames");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"WASAPI Exclusive Mode fehlgeschlagen für '{device.FriendlyName}': {ex.Message}");
                    }

                    result.Add(new DeviceInfo
                    {
                        Type = AudioDeviceType.WASAPI,
                        Name = device.FriendlyName,
                        IsAvailable = true,
                        PreferredInputChannels = props.Channels,
                        PreferredSampleRate = props.SampleRate,
                        PreferredBitDepth = props.BitsPerSample != 0 ? props.BitsPerSample : 32,
                        PreferredBufferSize = bufferSizeFrames // null, wenn Exclusive Mode nicht möglich
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WASAPI-Gerät '{device.FriendlyName}' übersprungen: {ex.Message}");
                }
            }

            return result;
        }
    }
}
