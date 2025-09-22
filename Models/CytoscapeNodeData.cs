using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public abstract class CytoscapeNodeData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("label")]
    public string Label { get; set; } = "";

    [JsonProperty("class")]
    public string Class { get; set; } = "";
}
