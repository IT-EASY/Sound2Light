using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Sound2Light.Settings
{
    public class AppSettings
    {

        // VU-Kalibrierungseinstellungen
        public class CalibrationSettings
        {
            public double SignalFrequencyHz { get; set; } = 1000; // 1kHz
            public double SignalAmplitude { get; set; } = 0.126; // ≈-18dBFS
            public CalibrationSignalType SignalType { get; set; } = CalibrationSignalType.Sine;
        }

        public enum CalibrationSignalType
        {
            Sine,
            Noise,
            Square
        }
    }
}