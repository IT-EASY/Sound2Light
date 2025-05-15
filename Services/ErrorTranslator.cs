// Sound2Light/Services/ErrorTranslator.cs
using Sound2Light.Common;

using System.Collections.Generic;

namespace Sound2Light.Services
{
    public static class ErrorTranslator
    {
        private static readonly Dictionary<int, string> _errorMessages = new()
        {
            // Allgemeine Fehler (1xxx)
            { ErrorCodes.GENERIC_ERROR, "Ein unerwarteter Fehler im Bereich des Capture Device ist aufgetreten." },

            // ASIO-Fehler (11xx)
            { ErrorCodes.ASIO_ERROR, "Ein ASIO-Fehler ist aufgetreten." },
            { ErrorCodes.ASIO_NO_DRIVERS, "Keine ASIO-Treiber installiert oder erkannt." },
            { ErrorCodes.ASIO_DRIVER_NOT_INIT, "ASIO-Treiber nicht initialisiert." },
            { ErrorCodes.ASIO_DRIVER_INIT_FAILED, "ASIO-Treiber konnte nicht initialisiert werden." },
            { ErrorCodes.ASIO_BUFFER_SIZE_UNSUPPORTED, "Nicht unterstützte Buffer-Größe für ASIO-Treiber." },
            { ErrorCodes.ASIO_PLAYBACK_FAILED, "ASIO-Aufnahmefehler während der Wiedergabe." },
            { ErrorCodes.ASIO_STOP_FAILED, "Fehler beim Stoppen der ASIO-Aufnahme." },

            // WASAPI-Fehler (12xx)
            { ErrorCodes.WASAPI_ERROR, "Ein WASAPI-Fehler ist aufgetreten." },
            { ErrorCodes.WASAPI_NO_DEVICES, "Keine WASAPI-Aufnahmegeräte gefunden." },
            { ErrorCodes.WASAPI_NOT_INIT, "WASAPI nicht initialisiert." },
            { ErrorCodes.WASAPI_INIT_FAILED, "WASAPI konnte nicht initialisiert werden." },
            { ErrorCodes.WASAPI_DEVICE_ACCESS_DENIED, "Zugriff auf WASAPI-Gerät verweigert." },
            { ErrorCodes.WASAPI_RECORDING_FAILED, "WASAPI-Aufnahmefehler während der Aufzeichnung." },
            { ErrorCodes.WASAPI_STOP_FAILED, "Fehler beim Stoppen der WASAPI-Aufnahme." },

            // Audio-Verarbeitung (20xx)
            { ErrorCodes.AUDIO_PROCESSING_ERROR, "Fehler bei der Audio-Datenverarbeitung." }
        };

        public static string GetMessage(int errorCode)
        {
            if (_errorMessages.TryGetValue(errorCode, out var message))
                return message;

            return _errorMessages[ErrorCodes.GENERIC_ERROR];
        }
    }
}