using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Node data describing a software license associated with a component.
/// </summary>
public class LicenseNodeData : CytoscapeNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    /// <summary>
    /// Reference URL to the license text or SPDX entry.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; } = "";
}
