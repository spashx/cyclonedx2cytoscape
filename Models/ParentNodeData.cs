namespace Cdx2Cyto.Models;

/// <summary>
/// Node data representing a parent (compound) node to group related child nodes.
/// </summary>
public class ParentNodeData : CytoscapeNodeData
{
    /// <summary>
    /// Creates a parent node data with a given id and label.
    /// </summary>
    public ParentNodeData(string id, string label)
    {
        Id = id;
        Label = label;
    }
}
