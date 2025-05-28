namespace Sound2Light.Startup
{
    public interface ISystemBootstrapper
    {
        /// <summary>
        /// Führt die vollständige Initialisierung beim App-Start durch.
        /// </summary>
        void Run();
    }
}
