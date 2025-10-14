using CdxViz.Models.Common;
using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeGraph : IGraph
{
    [JsonPropertyName("elements")]
    public IElements Elements { get; set; } = new CytoscapeElements();
}
