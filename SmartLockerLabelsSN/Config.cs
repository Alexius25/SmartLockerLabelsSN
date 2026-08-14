using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace SmartLockerLabelsSN;

[Menu("Smart Locker Labels")]
public class Config : ConfigFile
{
    [Slider("Sign discovery distance", 0, 10, DefaultValue = 2.5f, Step = 0.1f, Format =  "{0:F2}")]
    public float SignDiscoveryDistance { get; set; } = 2.5f;
    
    [Toggle("Display messages")]
    public bool DisplayMessages { get; set; } = true;
}