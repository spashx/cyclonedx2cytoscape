using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class ComponentNodeData : CytoscapeNodeData
{
    public ComponentNodeData()
    {
        Class = "component";
    }

    [JsonProperty("type")]
    public string Type { get; set; } = "";

    [JsonProperty("version")]
    public string Version { get; set; } = "";
    
    [JsonProperty("group")]
    public string Group { get; set; } = "";
    
    [JsonProperty("severity")]
    public string Severity { get; set; } = "none";
    
    [JsonProperty("topParent")]
    public bool IsTopParent { get; set; } = false;
}
