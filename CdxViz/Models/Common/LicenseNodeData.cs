using System.Text.Json.Serialization;

namespace CdxViz.Models.Common;

public class LicenseNodeData : BaseNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}
