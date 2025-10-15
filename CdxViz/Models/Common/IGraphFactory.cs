using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CdxViz.Models.Common
{
    public interface IGraphFactory
    {
        IGraph CreateGraph();

        INode CreateNode();

        ILink CreateLink();
    }
}
