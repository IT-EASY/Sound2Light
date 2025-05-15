using Sound2Light.Settings;

using System;

namespace Sound2Light.Services.Audio.Disabled
{
    /// <summary>
    /// Thread-sicherer Ringbuffer für Audio-Daten mit AppSettings-Integration
    /// </summary>
    public class AudioRingBuffer
    {
        private readonly float[] _buffer;
        private int _writeIndex;
        private int _readIndex;
        private int _availableSamples;
        private readonly object _lock = new();

        // Diagnoseparameter
        public int Capacity { get; }
        public int TotalWritten { get; private set; }
        public int TotalRead { get; private set; }

        /// <summary>
        /// Initialisiert den Buffer basierend auf AppSettings
        /// </summary>
        public AudioRingBuffer(AppSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            // Buffer-Größe berechnen: FFT-Größe × Multiplikator × Kanäle
            Capacity = settings.RingBuffer.FFTSize
                     * settings.RingBuffer.BufferMultiplier
                     * settings.Audio.Channels;

            _buffer = new float[Capacity];
        }

        /// <summary>
        /// Schreibt Daten in den Buffer (thread-safe)
        /// </summary>
        public void Append(float[] data)
        {
            if (data == null || data.Length == 0) return;

            lock (_lock)
            {
                int freeSpace = Capacity - _availableSamples;
                int samplesToWrite = Math.Min(data.Length, freeSpace);

                // 1. Segment: Vom WriteIndex bis Buffer-Ende
                int firstSegmentLength = Math.Min(samplesToWrite, Capacity - _writeIndex);
                Array.Copy(data, 0, _buffer, _writeIndex, firstSegmentLength);

                // 2. Segment: Wrap-around zum Buffer-Anfang
                if (samplesToWrite > firstSegmentLength)
                {
                    int secondSegmentLength = samplesToWrite - firstSegmentLength;
                    Array.Copy(data, firstSegmentLength, _buffer, 0, secondSegmentLength);
                }

                _writeIndex = (_writeIndex + samplesToWrite) % Capacity;
                _availableSamples += samplesToWrite;
                TotalWritten += samplesToWrite;

                // ReadIndex anpassen bei Überlauf
                if (data.Length > freeSpace)
                {
                    int overflow = data.Length - freeSpace;
                    _readIndex = (_readIndex + overflow) % Capacity;
                    _availableSamples = Capacity; // Buffer voll
                }
            }
        }

        /// <summary>
        /// Liest Daten aus dem Buffer (thread-safe)
        /// </summary>
        public int Read(float[] destination, int offset, int count)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (offset + count > destination.Length)
                throw new ArgumentOutOfRangeException("Zielarray zu klein");

            lock (_lock)
            {
                int samplesToRead = Math.Min(count, _availableSamples);
                if (samplesToRead == 0) return 0;

                // 1. Segment: Vom ReadIndex bis Buffer-Ende
                int firstSegmentLength = Math.Min(samplesToRead, Capacity - _readIndex);
                Array.Copy(_buffer, _readIndex, destination, offset, firstSegmentLength);

                // 2. Segment: Wrap-around zum Buffer-Anfang
                if (samplesToRead > firstSegmentLength)
                {
                    int secondSegmentLength = samplesToRead - firstSegmentLength;
                    Array.Copy(_buffer, 0, destination, offset + firstSegmentLength, secondSegmentLength);
                }

                _readIndex = (_readIndex + samplesToRead) % Capacity;
                _availableSamples -= samplesToRead;
                TotalRead += samplesToRead;

                return samplesToRead;
            }
        }

        /// <summary>
        /// Setzt den Buffer zurück (thread-safe)
        /// </summary>
        public void Clear(bool hardReset = false)
        {
            lock (_lock)
            {
                _writeIndex = 0;
                _readIndex = 0;
                _availableSamples = 0;
                TotalWritten = 0;
                TotalRead = 0;

                if (hardReset) Array.Clear(_buffer, 0, Capacity);
            }
        }
    }
}