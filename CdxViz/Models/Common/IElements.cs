using CdxViz.Models.Common;

namespace CdxViz.Models.Common;

public interface IElements
{
    List<ILink> Links { get; }
    List<INode> Nodes { get; }

    void AddLink(ILink edge);

    void AddNode(INode node);
}
