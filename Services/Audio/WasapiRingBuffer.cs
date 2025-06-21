using Sound2Light.Contracts.Audio;
using Sound2Light.Contracts.Services.Audio;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sound2Light.Services.Audio
{
    /// <summary>
    /// Thread-sicherer Ringbuffer für Stereo-Audio (L/R interleaved, float), mit Multi-Consumer-Unterstützung.
    /// </summary>
    public class WasapiRingBuffer : IAudioAnalysisBuffer, IWasapiRingBuffer
    {
        private readonly float[] _buffer;
        private int _writePos;
        private int _frameCount;
        private readonly int _frameCapacity;
        private readonly object _sync = new();

        private readonly Dictionary<string, int> _consumerReadIndices = new();

        public int Capacity => _frameCapacity;

        public int Count
        {
            get { lock (_sync) { return _frameCount; } }
        }

        public WasapiRingBuffer(int frameCapacity)
        {
            if (frameCapacity < 1) throw new ArgumentOutOfRangeException(nameof(frameCapacity));
            _frameCapacity = frameCapacity;
            _buffer = new float[frameCapacity * 2]; // Interleaved L/R!
            _writePos = 0;
            _frameCount = 0;
        }

        public void Write(float[] data, int offset, int frameCount)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset + frameCount * 2 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            lock (_sync)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    int writeIndex = (_writePos % _frameCapacity) * 2;
                    _buffer[writeIndex] = data[offset + i * 2];
                    _buffer[writeIndex + 1] = data[offset + i * 2 + 1];

                    _writePos = (_writePos + 1) % _frameCapacity;
                    if (_frameCount < _frameCapacity)
                        _frameCount++;
                }
            }
            //Debug.WriteLine($"[Write] writePos={_writePos}, frameCount={frameCount}");

        }

        public bool TryGetLatestFrames(float[] left, float[] right, int frameCount)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (frameCount < 1 || left.Length < frameCount || right.Length < frameCount)
                throw new ArgumentOutOfRangeException(nameof(frameCount));

            lock (_sync)
            {
                if (_frameCount < frameCount)
                {
                    return false;
                }

                int start = (_writePos - frameCount + _frameCapacity) % _frameCapacity;
                for (int i = 0; i < frameCount; i++)
                {
                    int readIndex = ((start + i) % _frameCapacity) * 2;
                    left[i] = _buffer[readIndex];
                    right[i] = _buffer[readIndex + 1];
                }
                return true;
            }
        }

        public void RegisterConsumer(string name)
        {
            lock (_sync)
            {
                if (!_consumerReadIndices.ContainsKey(name))
                {
                    _consumerReadIndices[name] = _writePos;
                    Debug.WriteLine($"[RingBuffer] Consumer '{name}' registriert bei WritePos={_writePos}");
                }
            }
        }

        public void UnregisterConsumer(string name)
        {
            lock (_sync)
            {
                _consumerReadIndices.Remove(name);
            }
        }

        public bool HasNewSamplesFor(string name, int frameCount)
        {
            lock (_sync)
            {
                if (!_consumerReadIndices.TryGetValue(name, out int readPos))
                    return false;
                int available = (_writePos - readPos + _frameCapacity) % _frameCapacity;
                return available >= frameCount;
            }
        }

        public bool CopySamplesFor(string name, float[] left, float[] right, int frameCount)
        {
            if (left == null || right == null)
                throw new ArgumentNullException();

            lock (_sync)
            {
                if (!_consumerReadIndices.TryGetValue(name, out int readPos))
                    return false;

                int available = (_writePos - readPos + _frameCapacity) % _frameCapacity;
                if (available < frameCount)
                    return false;

                for (int i = 0; i < frameCount; i++)
                {
                    int readIndex = ((readPos + i) % _frameCapacity) * 2;
                    left[i] = _buffer[readIndex];
                    right[i] = _buffer[readIndex + 1];
                }

                _consumerReadIndices[name] = (readPos + frameCount) % _frameCapacity;
                //Debug.WriteLine($"[CopySamplesFor] writePos={_writePos}, readPos={readPos}, available={available}, required={frameCount}");

                return true;
            }
        }
    }
}
