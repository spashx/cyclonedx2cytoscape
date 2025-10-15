using CdxViz.Models.Common;
using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeLink : ILink
{
    [JsonPropertyName("data")]
    public BaseLinkData Data { get; set; } = new();
}
