namespace Sound2Light.Contracts.Audio
{
    public interface IWasapiRingBuffer
    {
        int Capacity { get; }
        int Count { get; }
        void Write(float[] data, int offset, int frameCount);
        bool CopyLatestSamplesTo(float[] left, float[] right, int frameCount);
    }
}
