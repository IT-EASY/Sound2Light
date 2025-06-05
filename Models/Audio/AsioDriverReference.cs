namespace Sound2Light.Models.Audio;

public class AsioDriverReference
{
    public string Name { get; set; } = string.Empty;
    public Guid Clsid { get; set; }
    public string? DllPath { get; set; }
}