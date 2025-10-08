using System.Text.Json.Serialization;

namespace cdxviz.Models.Bom;

public class SimpleLicense
{
    [JsonPropertyName("license")]
    public LicenseContent? License { get; set; }
}
