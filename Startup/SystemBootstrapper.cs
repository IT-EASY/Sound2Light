// Sound2Light/Startup/SystemBootstrapper.cs
using Microsoft.Extensions.DependencyInjection;
using Sound2Light.Config;
using Sound2Light.Settings;
using Sound2Light.Models.Audio;
using Sound2Light.Services.Audio;
using System;
using System.Collections.Generic;
using Sound2Light.Views.Windows;

namespace Sound2Light.Startup
{
    public class SystemBootstrapper : ISystemBootstrapper
    {
        private readonly IAppConfigurationService _configService;

        public SystemBootstrapper(IAppConfigurationService configService)
        {
            _configService = configService;
        }

        public void Run()
        {
            var config = _configService.Settings;
            var selectedDevice = CaptureDeviceSelector.SelectValidDevice(config);

            if (selectedDevice != null)
            {
                config.PreferredCaptureDevice = new CaptureDeviceConfig
                {
                    Name = selectedDevice.Type == 0 ? selectedDevice.Name : selectedDevice.DisplayName,
                    Type = selectedDevice.Type,
                    SampleRate = selectedDevice.PreferredSampleRate ?? 0,
                    BitDepth = selectedDevice.PreferredBitDepth ?? 0,
                    Channels = selectedDevice.PreferredInputChannels ?? 0,
                    BufferSize = selectedDevice.PreferredBufferSize ?? 0
                };
            }
            else
            {
                // Kein Gerät → Setup-Fenster öffnen
                var setup = new SetupCaptureWindow();
                setup.ShowDialog();
            }
        }
    }
}