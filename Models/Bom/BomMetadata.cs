using System.Text.Json.Serialization;

namespace cdxviz.Models.Bom;

public class BomMetadata
{
    [JsonPropertyName("component")]
    public SimpleComponent? Component { get; set; }
}
