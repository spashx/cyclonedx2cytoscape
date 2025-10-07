using CdxViz.Models.Cytoscape;
using Newtonsoft.Json;

namespace CdxViz.Models;

public class LicenseNodeData : CytoscapeNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    [JsonProperty("url")]
    public string Url { get; set; } = "";
}
