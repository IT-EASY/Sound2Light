using System.Collections.Generic;

namespace Sound2Light.Helpers.Audio
{
    public static class AsioSampleTypeHelper
    {
        private static readonly Dictionary<int, string> _sampleTypeDescriptions = new()
        {
            { 0,  "16-bit Integer, Big Endian" },
            { 1,  "24-bit Integer, Big Endian" },
            { 2,  "32-bit Integer, Big Endian" },
            { 3,  "32-bit Float, Big Endian" },
            { 4,  "64-bit Float, Big Endian" },
            { 8,  "32-bit Integer, Big Endian, 16-bit data" },
            { 9,  "32-bit Integer, Big Endian, 18-bit data" },
            { 10, "32-bit Integer, Big Endian, 20-bit data" },
            { 11, "32-bit Integer, Big Endian, 24-bit data" },
            { 16, "16-bit Integer, Little Endian" },
            { 17, "24-bit Integer, Little Endian" },
            { 18, "32-bit Integer, Little Endian" },
            { 19, "32-bit Float, Little Endian" },
            { 20, "64-bit Float, Little Endian" },
            { 24, "32-bit Integer, Little Endian, 16-bit data" },
            { 25, "32-bit Integer, Little Endian, 18-bit data" },
            { 26, "32-bit Integer, Little Endian, 20-bit data" },
            { 27, "32-bit Integer, Little Endian, 24-bit data" },
            { 32, "DSD 1-bit Integer, Little Endian, 8-bit data" },
            { 33, "DSD 1-bit Integer, Big Endian, 8-bit data" },
            { 40, "DSD 1-bit Integer, Non-Endian, 8-bit data" }
        };

        public static string GetDescription(int? sampleType)
        {
            if (sampleType == null) return "-";
            if (_sampleTypeDescriptions.TryGetValue(sampleType.Value, out var desc))
                return desc;
            return $"Unbekannt ({sampleType.Value})";
        }
    }
}
