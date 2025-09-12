using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Base class for all Cytoscape node data payloads. Provides common fields (id, label, class).
/// </summary>
public abstract class CytoscapeNodeData
{
    /// <summary>
    /// Unique node identifier used for edge source/target references.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    /// <summary>
    /// Human readable label displayed in the graph.
    /// </summary>
    [JsonProperty("label")]
    public string Label { get; set; } = "";

    /// <summary>
    /// Semantic class name used by Cytoscape stylesheets.
    /// </summary>
    [JsonProperty("class")]
    public string Class { get; set; } = "";
}
