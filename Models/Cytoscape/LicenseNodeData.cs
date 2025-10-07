using CdxViz.Models.Cytoscape;
using Newtonsoft.Json;

namespace cdxviz.Models.Cytoscape;

public class LicenseNodeData : AbstractCytoscapeNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    [JsonProperty("url")]
    public string Url { get; set; } = "";
}
