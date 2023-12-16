using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.ModelsChain
{
    public class BlockChain
    {
        public string ChainDescription { get; set; }
        public DateTime GenesisDate { get; set; }
        public List<DataBlock> NodesList { get; set; }
        public Guid UniqueId { get; set; }
    }
}
