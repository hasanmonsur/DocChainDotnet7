using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.Models
{
    [Serializable]
    public class NetworkNode
    {
        public string Description { get; set; }
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
    }
}
