using System.Text.Json.Serialization;

namespace CdxViz.Models.Bom;

public class SimpleLicense
{
    [JsonPropertyName("license")]
    public LicenseContent? License { get; set; }
}
