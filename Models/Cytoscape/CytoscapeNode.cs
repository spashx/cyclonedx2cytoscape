using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeNode
{
    [JsonPropertyName("data")]
    public AbstractCytoscapeNodeData Data { get; set; }

    public CytoscapeNode(AbstractCytoscapeNodeData data)
    {
        Data = data;
    }
}
