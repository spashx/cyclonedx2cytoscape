using Newtonsoft.Json;

namespace cdxviz.Models.Bom;

public class LicenseContent
{
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("url")]
    public string? Url { get; set; }
}
