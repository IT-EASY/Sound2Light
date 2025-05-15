// Sound2Light/Services/Audio/IAudioDeviceManager.cs  
using Sound2Light.Models;
using Sound2Light.Common;
using System.Collections.Generic;

namespace Sound2Light.Services.Audio
{
    public interface IAudioDeviceManager
    {
        OperationResult<IEnumerable<AudioDeviceInfo>> GetAsioDevices();
        OperationResult<IEnumerable<AudioDeviceInfo>> GetWasapiDevices();
    }
}