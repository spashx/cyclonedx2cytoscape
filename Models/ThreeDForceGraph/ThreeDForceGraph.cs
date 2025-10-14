using CdxViz.Models.Common;
using System.Text.Json.Serialization;

namespace CdxViz.Models.ThreeDForceGraph;

public class ThreeDForceGraph : IGraph
{
    public IElements Elements { get; set; } = new ThreeDElements();
}
