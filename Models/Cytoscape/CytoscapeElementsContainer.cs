using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeElements
{
    private readonly List<CytoscapeNode> nodes = new();
    private readonly List<CytoscapeEdge> edges = new();

    [JsonPropertyName("nodes")]
    public List<CytoscapeNode> Nodes => nodes;

    [JsonPropertyName("edges")]
    public List<CytoscapeEdge> Edges => edges;

    public void AddNode(CytoscapeNode node)
    {
        nodes.Add(node);
    }

    public void AddEdge(CytoscapeEdge edge)
    {
        edges.Add(edge);
    }
}
