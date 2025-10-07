using Newtonsoft.Json;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeGraph
{
    [JsonProperty("elements")]
    public CytoscapeElements Elements { get; set; } = new();
}
