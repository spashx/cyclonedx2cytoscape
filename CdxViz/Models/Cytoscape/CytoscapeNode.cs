using CdxViz.Models.Common;
using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeNode : INode
{
    [JsonPropertyName("data")]
    public BaseNodeData Data { get; set; }
}
