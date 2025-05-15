// Sound2Light/Common/ErrorEventArgs.cs
namespace Sound2Light.Common
{
    public class ErrorEventArgs : EventArgs
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}