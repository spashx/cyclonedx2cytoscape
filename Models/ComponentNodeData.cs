using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Node data describing a software component (e.g., dependency or package).
/// </summary>
public class ComponentNodeData : CytoscapeNodeData
{
    public ComponentNodeData()
    {
        Class = "component";
    }

    /// <summary>
    /// Component type (e.g., library, container, module).
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "";

    /// <summary>
    /// Component version string.
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; } = "";
    
    /// <summary>
    /// Optional group or namespace.
    /// </summary>
    [JsonProperty("group")]
    public string Group { get; set; } = "";
    
    /// <summary>
    /// Aggregate severity computed from contained vulnerabilities (or 'none').
    /// </summary>
    [JsonProperty("severity")]
    public string Severity { get; set; } = "none";
    
    /// <summary>
    /// Indicates this component is the root/top-level parent in the graph.
    /// </summary>
    [JsonProperty("topParent")]
    public bool IsTopParent { get; set; } = false;
}
