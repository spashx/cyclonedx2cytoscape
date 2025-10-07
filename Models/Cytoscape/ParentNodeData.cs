using CdxViz.Models.Cytoscape;

namespace cdxviz.Models.Cytoscape;

public class ParentNodeData : AbstractCytoscapeNodeData
{
    public ParentNodeData(string id, string label)
    {
        Id = id;
        Label = label;
    }
}
