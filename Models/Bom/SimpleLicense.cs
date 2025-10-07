using Newtonsoft.Json;

namespace cdxviz.Models.Bom;

public class SimpleLicense
{
    [JsonProperty("license")]
    public LicenseContent? License { get; set; }
}
