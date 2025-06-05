using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text; // Dieser using war notwendig

using Sound2Light.Interop.ASIO;
using Sound2Light.Models.Audio;

namespace Sound2Light.Services.Devices
{
    public static class AsioDriverProbe
    {
        // Import Windows API to get main window handle
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public static AudioDevice? TryCreateDevice(AsioDriverReference reference)
        {
            if (!File.Exists(reference.DllPath))
            {
                Debug.WriteLine($"[ASIO-Probe] DLL nicht gefunden: {reference.DllPath}");
                return null;
            }

            Type? comType;
            try
            {
                comType = Type.GetTypeFromCLSID(reference.Clsid, true);
                if (comType == null)
                {
                    Debug.WriteLine($"[ASIO-Probe] Kein COM-Typ für CLSID {reference.Clsid}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ASIO-Probe] COM-Typ konnte nicht erzeugt werden: {ex.Message}");
                return null;
            }

            IASIODriver? driver = null;
            object? comObject = null;

            try
            {
                comObject = Activator.CreateInstance(comType);
                if (comObject == null)
                {
                    Debug.WriteLine($"[ASIO-Probe] COM-Objekt für {reference.Name} konnte nicht erstellt werden.");
                    return null;
                }

                // Direkter Cast statt QueryInterface
                driver = comObject as IASIODriver;
                if (driver == null)
                {
                    Debug.WriteLine($"[ASIO-Probe] Objekt implementiert IASIODriver nicht");
                    return null;
                }

                // Verwende echtes Fenster-Handle statt IntPtr.Zero
                IntPtr mainWindowHandle = GetActiveWindow();
                int hr = driver.Init(mainWindowHandle);
                Debug.WriteLine($"[ASIO-Probe] Init({reference.Name}) → HRESULT: {hr}, HWND: 0x{mainWindowHandle.ToInt64():X}");

                if (hr == 0) // 0 = ASIOTrue (Erfolg)
                {
                    driver.GetChannels(out int inCh, out int outCh);
                    driver.GetBufferSize(out int minBuf, out int maxBuf, out int preferredBuf, out int gran);
                    driver.GetSampleRate(out double rate);

                    Debug.WriteLine($"[ASIO-Probe] {reference.Name} → OK, SR: {rate} Hz, In: {inCh}, Buffer: {preferredBuf}");

                    return new AudioDevice
                    {
                        Name = reference.Name,
                        Type = AudioDeviceType.Asio,
                        IsAvailable = true,
                        SampleRate = (int)Math.Round(rate),
                        BitDepth = 32, // ASIO verwendet immer 32-bit Float
                        ChannelCount = inCh,
                        BufferSize = preferredBuf
                    };
                }
                else
                {
                    // Versuche Fehlermeldung abzurufen
                    try
                    {
                        var sb = new StringBuilder(256); // Korrektur: System.Text.StringBuilder
                        driver.GetErrorMessage(sb);
                        Debug.WriteLine($"[ASIO-Probe] Fehler: {sb}");
                    }
                    catch { /* Ignoriere Fehler beim Abrufen der Fehlermeldung */ }

                    Debug.WriteLine($"[ASIO-Probe] Init() schlug fehl für {reference.Name} (hr={hr})");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ASIO-Probe] Ausnahme bei Initialisierung von '{reference.Name}': {ex}");
            }
            finally
            {
                // Richtiges COM-Cleanup
                if (driver != null)
                {
                    try
                    {
                        // Wichtig: Erst den Treiber stoppen wenn nötig
                        driver.Stop();
                        driver.DisposeBuffers();
                    }
                    catch { /* Ignoriere */ }
                }

                if (comObject != null)
                {
                    try
                    {
                        Marshal.ReleaseComObject(comObject);
                        Debug.WriteLine($"[ASIO-Probe] COM-Objekt für '{reference.Name}' freigegeben.");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ASIO-Probe] Fehler beim Freigeben des COM-Objekts: {ex.Message}");
                    }
                }
            }

            Debug.WriteLine($"[ASIO] {reference.Name} → nicht verfügbar / konnte nicht initialisiert werden");
            return new AudioDevice
            {
                Name = reference.Name,
                Type = AudioDeviceType.Asio,
                IsAvailable = false,
                SampleRate = 0,
                BitDepth = 0,
                ChannelCount = 0,
                BufferSize = 0
            };
        }
    }
}