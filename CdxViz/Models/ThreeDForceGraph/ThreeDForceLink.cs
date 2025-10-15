using CdxViz.Models.Common;
using CdxViz.Models.Cytoscape;
using System.Text.Json.Serialization;

namespace CdxViz.Models.ThreeDForceGraph;

public class ThreeDForceLink : ILink
{
    public BaseLinkData Data { get; set; }
}