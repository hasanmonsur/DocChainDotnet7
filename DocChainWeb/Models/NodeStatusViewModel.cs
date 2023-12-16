using DocChainWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.Models
{
    public class NodeStatusViewModel
    {
        public List<NetworkNode> Nodes { get; set; }
        public BlockChain Chain { get; set; }
    }
}
