using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Sound2Light.Services.Audio
{ 
    public class MonoAnalysisBuffer
    {
    // *** KONSTANTEN ***
    private const int BufferSize = 48000; // 1s @ 48kHz

    // *** FELDER ***
    private readonly float[] _buffer; // Ringbuffer für Mono-Samples
    private int _writeIndex; // globaler Schreibindex (Frames)

    // Jeder Consumer hat einen eigenen atomaren Leseindex
    private readonly ConcurrentDictionary<string, int> _readIndices = new();

    // Synchronisierung: WriteIndex wird nur von FillFromStereoBuffer geschrieben, sonst gelesen

    // *** KONSTRUKTOR ***
    public MonoAnalysisBuffer()
    {
        _buffer = new float[BufferSize];
        _writeIndex = 0;
    }

    /// <summary>
    /// Registrierung eines neuen Readers. Name MUSS eindeutig sein!
    /// </summary>
    public void RegisterConsumer(string consumerName)
    {
        if (!_readIndices.TryAdd(consumerName, _writeIndex))
            throw new InvalidOperationException($"Consumer '{consumerName}' bereits registriert.");
    }

    /// <summary>
    /// Austragung eines Consumers (z. B. beim Unload).
    /// </summary>
    public void UnregisterConsumer(string consumerName)
    {
        _readIndices.TryRemove(consumerName, out _);
    }

    /// <summary>
    /// Rückgabe: Wie viele Mono-Frames sind für diesen Consumer verfügbar?
    /// </summary>
    public int GetAvailableSamples(string consumerName)
    {
        if (!_readIndices.TryGetValue(consumerName, out var readIdx))
            return 0;

        int w = Volatile.Read(ref _writeIndex);
        int available = (w - readIdx + BufferSize) % BufferSize;
        return available;
    }

    /// <summary>
    /// Liefert bis zu count neue Mono-Samples für diesen Consumer.
    /// Gibt false zurück, wenn weniger als count Samples vorliegen.
    /// </summary>
    public bool GetSamples(string consumerName, float[] dest, int count)
    {
        if (dest == null) throw new ArgumentNullException(nameof(dest));
        if (count > dest.Length) throw new ArgumentOutOfRangeException(nameof(count));

        if (!_readIndices.TryGetValue(consumerName, out var readIdx))
            return false;

        int w = Volatile.Read(ref _writeIndex);
        int available = (w - readIdx + BufferSize) % BufferSize;
        if (available < count)
            return false;

        // Daten kopieren (Wrap berücksichtigen)
        for (int i = 0; i < count; i++)
        {
            int idx = (readIdx + i) % BufferSize;
            dest[i] = _buffer[idx];
        }
        // Reader-Index updaten
        _readIndices[consumerName] = (readIdx + count) % BufferSize;
        return true;
    }

    /// <summary>
    /// Füllt den MonoBuffer aus einem Stereobuffer (z. B. aus ASIO/WASAPI). 
    /// Muss regelmäßig vom Producer aufgerufen werden.
    /// left/right: Stereodaten, frameCount: wie viele Frames zu schreiben.
    /// 
    /// Die Methode mischt die Kanäle zu Mono (L+R)*0.5.
    /// </summary>
    public void FillFromStereoBuffer(float[] left, float[] right, int frameCount)
    {
        if (left == null || right == null) throw new ArgumentNullException();
        if (left.Length < frameCount || right.Length < frameCount) throw new ArgumentOutOfRangeException();

        int w = _writeIndex;
        for (int i = 0; i < frameCount; i++)
        {
            int idx = (w + i) % BufferSize;
            _buffer[idx] = 0.5f * (left[i] + right[i]);
        }
        // Atomarer Update nach erfolgreichem Copy!
        Interlocked.Exchange(ref _writeIndex, (w + frameCount) % BufferSize);
    }

    public void FillFromStereoBuffer_EnergyPreserving(float[] left, float[] right, int frameCount)
    {
        if (left == null || right == null) throw new ArgumentNullException();
        if (left.Length < frameCount || right.Length < frameCount) throw new ArgumentOutOfRangeException();

        int w = _writeIndex;
        for (int i = 0; i < frameCount; i++)
        {
            int idx = (w + i) % BufferSize;
            // Energieerhaltender Downmix mit RMS und Vorzeichen
            _buffer[idx] = MathF.Sqrt((left[i] * left[i] + right[i] * right[i]) / 2f) * MathF.Sign(left[i] + right[i]);
        }
        Interlocked.Exchange(ref _writeIndex, (w + frameCount) % BufferSize);
    }
        /// <summary>
        /// Optional: Aktueller Bufferfüllstand (gesamt, nicht pro Consumer).
        /// </summary>
        public int CurrentFillLevel
    {
        get
        {
            // Minimaler Abstand zwischen WriteIndex und allen Readern (gibt min. zurück)
            int w = Volatile.Read(ref _writeIndex);
            int min = BufferSize;
            foreach (var r in _readIndices.Values)
            {
                int dist = (w - r + BufferSize) % BufferSize;
                if (dist < min) min = dist;
            }
            return min;
        }
    }
}
}
