using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Data payload for a Cytoscape edge connecting two nodes.
/// </summary>
public class CytoscapeEdgeData
{
    /// <summary>
    /// Unique edge identifier.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    /// <summary>
    /// Source node id.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; set; } = "";

    /// <summary>
    /// Target node id.
    /// </summary>
    [JsonProperty("target")]
    public string Target { get; set; } = "";
    
    /// <summary>
    /// Semantic class name used in styling.
    /// </summary>
    [JsonProperty("class")]
    public string Class { get; set; } = "";
}
