// Sound2Light/Common/ErrorCodes.cs
namespace Sound2Light.Common
{
    public static class ErrorCodes
    {
        // Allgemeine Fehler (1xxx)
        public const int GENERIC_ERROR = 1000;

        // ASIO-Fehler (11xx)
        public const int ASIO_ERROR = 1100;
        public const int ASIO_NO_DRIVERS = 1101;
        public const int ASIO_DRIVER_NOT_INIT = 1102;
        public const int ASIO_DRIVER_INIT_FAILED = 1103;
        public const int ASIO_BUFFER_SIZE_UNSUPPORTED = 1110;
        public const int ASIO_PLAYBACK_FAILED = 1120;
        public const int ASIO_STOP_FAILED = 1121;

        // WASAPI-Fehler (12xx)
        public const int WASAPI_ERROR                   = 1200;
        public const int WASAPI_NO_DEVICES              = 1201;
        public const int WASAPI_NOT_INIT                = 1202;
        public const int WASAPI_INIT_FAILED             = 1203;
        public const int WASAPI_DEVICE_ACCESS_DENIED    = 1204;
        public const int WASAPI_RECORDING_FAILED        = 1221;
        public const int WASAPI_STOP_FAILED             = 1223;

        // Audio-Verarbeitung (20xx)
        public const int AUDIO_PROCESSING_ERROR         = 2001;
    }
}