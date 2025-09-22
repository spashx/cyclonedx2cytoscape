using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class CytoscapeEdgeData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("source")]
    public string Source { get; set; } = "";

    [JsonProperty("target")]
    public string Target { get; set; } = "";
    
    [JsonProperty("class")]
    public string Class { get; set; } = "";
}
