using System.Text.Json.Serialization;

namespace CdxViz.Models.Bom;

public class BomMetadata
{
    [JsonPropertyName("component")]
    public SimpleComponent? Component { get; set; }
}
