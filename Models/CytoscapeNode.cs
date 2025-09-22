using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class CytoscapeNode
{
    [JsonProperty("data")]
    public CytoscapeNodeData Data { get; set; }

    public CytoscapeNode(CytoscapeNodeData data)
    {
        Data = data;
    }
}
