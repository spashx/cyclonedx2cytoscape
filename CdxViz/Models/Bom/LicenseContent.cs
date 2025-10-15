using System.Text.Json.Serialization;

namespace CdxViz.Models.Bom;

public class LicenseContent
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
