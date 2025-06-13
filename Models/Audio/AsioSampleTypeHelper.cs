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
            { 5,  "32-bit Integer, Big Endian, 16-bit data" },
            { 6,  "32-bit Integer, Big Endian, 18-bit data" },
            { 7,  "32-bit Integer, Big Endian, 20-bit data" },
            { 8,  "32-bit Integer, Big Endian, 24-bit data" },
            { 9,  "16-bit Integer, Little Endian" },
            { 10, "24-bit Integer, Little Endian" },
            { 11, "32-bit Integer, Little Endian" },
            { 18, "32-bit Float, Little Endian" },
            { 19, "64-bit Float, Little Endian" },
            { 20, "32-bit Integer, Little Endian, 16-bit data" },
            { 21, "32-bit Integer, Little Endian, 18-bit data" },
            { 22, "32-bit Integer, Little Endian, 20-bit data" },
            { 23, "32-bit Integer, Little Endian, 24-bit data" },
            { 24, "DSD 1-bit Integer, Little Endian, 8-bit data" },
            { 25, "DSD 1-bit Integer, Big Endian, 8-bit data" },
            { 26, "DSD 1-bit Integer, Non-Endian, 8-bit data" }
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
