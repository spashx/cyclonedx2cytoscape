using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class CytoscapeElements
{
    private readonly List<CytoscapeNode> nodes = new();
    private readonly List<CytoscapeEdge> edges = new();

    [JsonProperty("nodes", Order = 1)]
    public List<CytoscapeNode> Nodes => nodes;

    [JsonProperty("edges", Order = 2)]
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
