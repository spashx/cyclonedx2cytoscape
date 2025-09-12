using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

/// <summary>
/// Holds all Cytoscape elements (nodes and edges) for a graph instance.
/// </summary>
public class CytoscapeElements
{
    // Backing fields to keep collections mutable internally while exposing read-only style properties.
    private readonly List<CytoscapeNode> nodes = new();
    private readonly List<CytoscapeEdge> edges = new();

    /// <summary>
    /// All node entries in the graph. Serialized as 'nodes'.
    /// </summary>
    [JsonProperty("nodes", Order = 1)]
    public List<CytoscapeNode> Nodes => nodes;

    /// <summary>
    /// All edge entries in the graph. Serialized as 'edges'.
    /// </summary>
    [JsonProperty("edges", Order = 2)]
    public List<CytoscapeEdge> Edges => edges;

    /// <summary>
    /// Adds a node to the graph.
    /// </summary>
    public void AddNode(CytoscapeNode node)
    {
        nodes.Add(node);
    }

    /// <summary>
    /// Adds an edge to the graph.
    /// </summary>
    public void AddEdge(CytoscapeEdge edge)
    {
        edges.Add(edge);
    }
}
