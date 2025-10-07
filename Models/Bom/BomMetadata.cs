using Newtonsoft.Json;

namespace cdxviz.Models.Bom;

public class BomMetadata
{
    [JsonProperty("component")]
    public SimpleComponent? Component { get; set; }
}
