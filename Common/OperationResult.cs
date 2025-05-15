using Sound2Light.Services;

namespace Sound2Light.Common
{
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public T? Data { get; set; }

        // Automatische Übersetzung
        [Newtonsoft.Json.JsonIgnore]
        public string TranslatedMessage => ErrorTranslator.GetMessage(ErrorCode);

        // Optionale benutzerdefinierte Nachricht
        public string CustomMessage { get; set; } = string.Empty;
    }
}