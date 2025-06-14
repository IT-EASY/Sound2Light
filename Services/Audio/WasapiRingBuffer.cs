using Sound2Light.Contracts.Services.Audio;

using System;
using System.Diagnostics;

namespace Sound2Light.Services.Audio
{
    /// <summary>
    /// Thread-sicherer Ringbuffer für Stereo-Audio (L/R interleaved, float), implementiert IAudioAnalysisBuffer.
    /// </summary>
    public class WasapiRingBuffer : IAudioAnalysisBuffer
    {
        private readonly float[] _buffer;
        private int _writePos;
        private int _frameCount;
        private readonly int _frameCapacity;
        private readonly object _sync = new();

        public int Capacity => _frameCapacity;
        public int Count
        {
            get { lock (_sync) { return _frameCount; } }
        }

        /// <summary>
        /// Erzeugt einen neuen Buffer mit der gewünschten Frame-Anzahl (nicht Sample-Anzahl!).
        /// </summary>
        public WasapiRingBuffer(int frameCapacity)
        {
            if (frameCapacity < 1) throw new ArgumentOutOfRangeException(nameof(frameCapacity));
            _frameCapacity = frameCapacity;
            _buffer = new float[frameCapacity * 2]; // Interleaved L/R!
            _writePos = 0;
            _frameCount = 0;
        }

        /// <summary>
        /// Schreibe einen neuen Block von interleaved Stereo-Samples in den Puffer.
        /// FrameCount = wie viele Stereo-Frames (je Frame: L+R).
        /// </summary>
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
        }

        /// <summary>
        /// Liefert die letzten frameCount Frames (links/rechts getrennt). false, wenn nicht genug Daten.
        /// </summary>
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
    }
}
