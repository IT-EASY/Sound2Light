using System;
using System.Windows;
using Microsoft.Extensions.Logging;
using Sound2Light.Services.Audio;
using Sound2Light.Settings;

namespace Sound2Light
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}