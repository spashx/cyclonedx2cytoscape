using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class LicenseNodeData : CytoscapeNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    [JsonProperty("url")]
    public string Url { get; set; } = "";
}
