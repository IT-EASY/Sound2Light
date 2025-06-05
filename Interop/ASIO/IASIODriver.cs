using System;
using System.Runtime.InteropServices;

namespace Sound2Light.Interop.ASIO
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("7C57A7A0-FB6F-11D2-BDCA-00C04F8EC1C1")] // Diese GUID ist egal, weil wir das Interface nicht direkt via COM registrieren
    public interface IASIODriver
    {
        [PreserveSig] int Init(IntPtr sysHandle);
        void GetDriverName([MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder name);
        int GetDriverVersion();
        void GetErrorMessage([MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder message);
        int Start();
        int Stop();
        int GetChannels(out int numInputChannels, out int numOutputChannels);
        int GetBufferSize(out int minSize, out int maxSize, out int preferredSize, out int granularity);
        int CanSampleRate(double sampleRate);
        int GetSampleRate(out double sampleRate);
        int SetSampleRate(double sampleRate);
        int GetClockSources(IntPtr clocks, int numSources);
        int SetClockSource(int reference);
        int GetSamplePosition(out long samplePos, out long timeStamp);
        int GetChannelInfo(ref ASIOChannelInfo info);
        int CreateBuffers(IntPtr bufferInfos, int numChannels, int bufferSize, IntPtr callbacks);
        int DisposeBuffers();
        int ControlPanel();
        int Future(int selector, IntPtr opt);
        int OutputReady();
    }
}
