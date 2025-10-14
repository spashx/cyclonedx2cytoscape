using CdxViz.Models.Common;
using System.Text.Json.Serialization;

namespace CdxViz.Models.ThreeDForceGraph;

public class ThreeDForceNode : INode
{
    public BaseNodeData Data { get; set; }
}
