using Newtonsoft.Json;

namespace Cdx2Cyto.Models;

public class CytoscapeGraph
{
    [JsonProperty("elements")]
    public CytoscapeElements Elements { get; set; } = new();
}

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

public class CytoscapeNode
{
    [JsonProperty("data")]
    public CytoscapeNodeData Data { get; set; }

    public CytoscapeNode(CytoscapeNodeData data)
    {
        Data = data;
    }
}

public abstract class CytoscapeNodeData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("label")]
    public string Label { get; set; } = "";

    [JsonProperty("class")]
    public string Class { get; set; } = "";
}

public class ComponentNodeData : CytoscapeNodeData
{
    public ComponentNodeData()
    {
        Class = "component";
    }

    [JsonProperty("type")]
    public string Type { get; set; } = "";

    [JsonProperty("version")]
    public string Version { get; set; } = "";
    
    [JsonProperty("group")]
    public string Group { get; set; } = "";
    
    [JsonProperty("severity")]
    public string Severity { get; set; } = "none";
    
    [JsonProperty("topParent")]
    public bool IsTopParent { get; set; } = false;
}

public class VulnerabilityNodeData : CytoscapeNodeData
{
    public VulnerabilityNodeData()
    {
        Class = "vulnerability";
    }

    [JsonProperty("score")]
    public double? Score { get; set; }

    [JsonProperty("severity")]
    public string Severity { get; set; } = "";

    [JsonProperty("url")]
    public string Url { get; set; } = "";
}

public class CytoscapeEdge
{
    [JsonProperty("data")]
    public CytoscapeEdgeData Data { get; set; } = new();
}

public class CytoscapeEdgeData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("source")]
    public string Source { get; set; } = "";

    [JsonProperty("target")]
    public string Target { get; set; } = "";
    
    [JsonProperty("class")]
    public string Class { get; set; } = "";
}

public class ParentNodeData : CytoscapeNodeData
{
    public ParentNodeData(string id, string label)
    {
        Id = id;
        Label = label;
    }
}

public class LicenseNodeData : CytoscapeNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    [JsonProperty("url")]
    public string Url { get; set; } = "";
}

