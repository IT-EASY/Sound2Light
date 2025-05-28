using System.Windows;

namespace Sound2Light.Services.System
{
    public class ApplicationShutdownService : IApplicationShutdownService
    {
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}
