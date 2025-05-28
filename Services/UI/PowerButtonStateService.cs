using System.Windows.Threading;

using Sound2Light.Services.System;

namespace Sound2Light.Services.UI
{
    public class PowerButtonStateService : IPowerButtonStateService
    {
        private readonly IApplicationShutdownService _shutdownService;
        private readonly DispatcherTimer _relockTimer;

        private bool _isUnlocked = false;
        public bool IsUnlocked => _isUnlocked;

        public event EventHandler<bool>? UnlockStateChanged;
        public event EventHandler? ShutdownTriggered;

        public PowerButtonStateService(IApplicationShutdownService shutdownService)
        {
            _shutdownService = shutdownService;

            _relockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _relockTimer.Tick += (s, e) =>
            {
                _relockTimer.Stop();
                SetUnlocked(false);
            };
        }

        /// <summary>
        /// Direktes Entsperren (z. B. via Doppelklick).
        /// </summary>
        public void StartUnlockDirect()
        {
            if (!_isUnlocked)
            {
                SetUnlocked(true);
                _relockTimer.Start();
            }
        }

        /// <summary>
        /// Versucht das Herunterfahren der App, wenn entsperrt.
        /// </summary>
        public void AttemptShutdown()
        {
            if (_isUnlocked)
            {
                ShutdownTriggered?.Invoke(this, EventArgs.Empty);
                _shutdownService.Shutdown();
            }
        }

        private void SetUnlocked(bool unlocked)
        {
            _isUnlocked = unlocked;
            UnlockStateChanged?.Invoke(this, unlocked);
        }
    }
}
