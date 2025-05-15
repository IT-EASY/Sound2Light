// Sound2Light/Services/Audio/AudioDeviceManager.cs  
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Sound2Light.Common;
using Sound2Light.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sound2Light.Services.Audio
{
    public class AudioDeviceManager : IAudioDeviceManager
    {
        public OperationResult<IEnumerable<AudioDeviceInfo>> GetAsioDevices()
        {
            try
            {
                var drivers = AsioOut.GetDriverNames();
                return drivers.Length > 0
                    ? new OperationResult<IEnumerable<AudioDeviceInfo>>
                    {
                        Success = true,
                        Data = drivers.Select(d => new AudioDeviceInfo
                        {
                            ApiType = AudioApiType.ASIO,
                            TechnicalName = d
                        })
                    }
                    : new OperationResult<IEnumerable<AudioDeviceInfo>>
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.ASIO_NO_DRIVERS
                    };
            }
            catch
            {
                return new OperationResult<IEnumerable<AudioDeviceInfo>>
                {
                    Success = false,
                    ErrorCode = ErrorCodes.ASIO_ERROR,
                    Data = Enumerable.Empty<AudioDeviceInfo>()
                };
            }
        }

        public OperationResult<IEnumerable<AudioDeviceInfo>> GetWasapiDevices()
        {
            try
            {
                using var enumerator = new MMDeviceEnumerator();
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                    .Select(device => new AudioDeviceInfo
                    {
                        ApiType = AudioApiType.WASAPI,
                        TechnicalName = device.FriendlyName,
                        DisplayName = device.FriendlyName,
                        Channels = device.AudioClient?.MixFormat?.Channels ?? 2,
                        SampleRate = device.AudioClient?.MixFormat?.SampleRate ?? 44100,
                        BitDepth = device.AudioClient?.MixFormat?.BitsPerSample ?? 16
                    });

                return new OperationResult<IEnumerable<AudioDeviceInfo>>
                {
                    Success = true,
                    Data = devices
                };
            }
            catch 
            {
                return new OperationResult<IEnumerable<AudioDeviceInfo>>
                {
                    Success = false,
                    ErrorCode = ErrorCodes.WASAPI_ERROR,
                    Data = Enumerable.Empty<AudioDeviceInfo>()
                };
            }
        }
    }
}