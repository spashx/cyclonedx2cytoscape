using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Wrapper representing a Cytoscape edge. Holds edge data under a 'data' property.
/// </summary>
public class CytoscapeEdge
{
    /// <summary>
    /// Edge data (id, source, target, class) serialized as 'data'.
    /// </summary>
    [JsonProperty("data")]
    public CytoscapeEdgeData Data { get; set; } = new();
}
