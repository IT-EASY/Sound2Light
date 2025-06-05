using System;
using System.Runtime.InteropServices;

namespace Sound2Light.Interop.ASIO
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ASIOChannelInfo
    {
        public int channel;
        public bool isInput;
        public bool isActive;
        public int channelGroup;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
    }
}
