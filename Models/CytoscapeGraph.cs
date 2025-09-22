using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class CytoscapeGraph
{
    [JsonProperty("elements")]
    public CytoscapeElements Elements { get; set; } = new();
}
