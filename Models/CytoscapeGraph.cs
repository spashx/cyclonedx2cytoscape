using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Root graph object passed to Cytoscape. Wraps the collection of elements (nodes + edges).
/// </summary>
public class CytoscapeGraph
{
    /// <summary>
    /// Container grouping nodes and edges. Serialized under the JSON property 'elements'.
    /// </summary>
    [JsonProperty("elements")]
    public CytoscapeElements Elements { get; set; } = new();
}
