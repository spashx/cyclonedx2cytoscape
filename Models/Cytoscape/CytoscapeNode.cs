using Newtonsoft.Json;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeNode
{
    [JsonProperty("data")]
    public AbstractCytoscapeNodeData Data { get; set; }

    public CytoscapeNode(AbstractCytoscapeNodeData data)
    {
        Data = data;
    }
}
