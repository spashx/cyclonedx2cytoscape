using System.Text.Json.Serialization;

namespace CdxViz.Models.Common;

public class ComponentNodeData : BaseNodeData
{
    public ComponentNodeData()
    {
        Class = "component";
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("group")]
    public string Group { get; set; } = "";

    /// <summary>
    /// Severity of the most severe vulnerability associated with this component, if any
    /// </summary>
    [JsonPropertyName("severity")]
    public string Severity { get; set; } = "none";

    [JsonPropertyName("topParent")]
    public bool IsTopParent { get; set; } = false;
}
