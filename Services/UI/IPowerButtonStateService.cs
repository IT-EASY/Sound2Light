namespace Sound2Light.Services.UI
{
    public interface IPowerButtonStateService
    {
        /// <summary>
        /// True, wenn der Button aktuell entsperrt ist.
        /// </summary>
        bool IsUnlocked { get; }

        /// <summary>
        /// Wird ausgelöst, wenn sich der Entsperrstatus ändert.
        /// </summary>
        event EventHandler<bool>? UnlockStateChanged;

        /// <summary>
        /// Wird ausgelöst, wenn der Shutdown ausgelöst wurde.
        /// </summary>
        event EventHandler? ShutdownTriggered;

        void StartUnlockDirect();     // Rechtsklick gedrückt
        void AttemptShutdown();     // Linksklick (wenn entsperrt)
    }
}
