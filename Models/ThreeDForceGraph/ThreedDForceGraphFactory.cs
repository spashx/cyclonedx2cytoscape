using CdxViz.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdxViz.Models.ThreeDForceGraph
{
    public class ThreedDForceGraphFactory : IGraphFactory
    {
        public IGraph CreateGraph()
        {
            return new ThreeDForceGraph();
        }

        public ILink CreateLink()
        {
            return new ThreeDForceLink();
        }

        public INode CreateNode()
        {
            return new ThreeDForceNode();
        }
    }
}
