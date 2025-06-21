namespace Sound2Light.Contracts.Audio
{
    public interface IWasapiRingBuffer
    {
        int Capacity { get; }
        int Count { get; }

        void Write(float[] data, int offset, int frameCount);
        void RegisterConsumer(string name);
        void UnregisterConsumer(string name);
        bool HasNewSamplesFor(string name, int frameCount);
        bool CopySamplesFor(string name, float[] left, float[] right, int frameCount);
    }
}
