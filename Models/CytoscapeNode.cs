using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Wrapper object representing a Cytoscape node. Holds node data under a 'data' property.
/// </summary>
public class CytoscapeNode
{
    /// <summary>
    /// Arbitrary node data (id, label, type specific fields) serialized as 'data'.
    /// </summary>
    [JsonProperty("data")]
    public CytoscapeNodeData Data { get; set; }

    /// <summary>
    /// Creates a node wrapper for the provided node data.
    /// </summary>
    public CytoscapeNode(CytoscapeNodeData data)
    {
        Data = data;
    }
}
