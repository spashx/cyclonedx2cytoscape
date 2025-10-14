using CdxViz.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdxViz.Models.Cytoscape
{
    public class CystoscapeGraphFactory : IGraphFactory
    {
        public IGraph CreateGraph()
        {
            return new CytoscapeGraph();
        }

        public INode CreateNode()
        {
            return new CytoscapeNode();
        }

        public IElements CreateElements()
        {
            return new CytoscapeElements();
        }

        public ILink CreateLink()
        {
            return new CytoscapeLink();
        }
    }
}
