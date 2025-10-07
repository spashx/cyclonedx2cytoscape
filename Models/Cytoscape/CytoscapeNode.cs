using Newtonsoft.Json;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeNode
{
    [JsonProperty("data")]
    public CytoscapeNodeData Data { get; set; }

    public CytoscapeNode(CytoscapeNodeData data)
    {
        Data = data;
    }
}
