using CdxViz.Models.Cytoscape;

namespace CdxViz.Models;

public class ParentNodeData : CytoscapeNodeData
{
    public ParentNodeData(string id, string label)
    {
        Id = id;
        Label = label;
    }
}
