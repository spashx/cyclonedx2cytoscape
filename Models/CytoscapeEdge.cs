using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class CytoscapeEdge
{
    [JsonProperty("data")]
    public CytoscapeEdgeData Data { get; set; } = new();
}
