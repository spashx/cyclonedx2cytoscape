using System.Text.Json.Serialization;

namespace CdxViz.Models.Common;

public abstract class BaseNodeData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("class")]
    public string Class { get; set; } = "";
}
