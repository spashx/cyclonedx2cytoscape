using CdxViz.Models.Common;
using System.Text.Json.Serialization;

namespace CdxViz.Models.ThreeDForceGraph;

public class ThreeDElements : IElements
{
    private readonly List<INode> nodes = new();
    private readonly List<ILink> edges = new();

    [JsonPropertyName("nodes")]
    public List<INode> Nodes => nodes;

    [JsonPropertyName("links")]
    public List<ILink> Links => edges;

    public void AddNode(INode node)
    {
        nodes.Add(node);
    }

    public void AddLink(ILink edge)
    {
        edges.Add(edge);
    }
}
