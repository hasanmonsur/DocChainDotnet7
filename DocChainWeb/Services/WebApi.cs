namespace DocChainWeb.Services
{
    public class WebApi
    {
        private WebApi(string value) { Value = value; }

        public string Value { get; set; }

        public static WebApi GetNodesList { get { return new WebApi("/chain/GetNodesList"); } }
        public static WebApi WelcomeNode { get { return new WebApi("/chain/WelcomeNode"); } }
        public static WebApi NetworkUpdate { get { return new WebApi("/chain/NetworkNodesUpdate"); } }
        public static WebApi GetDataBlockBytes { get { return new WebApi("/chain/GetDataBlockBytes/"); } }
        public static WebApi StoreNewChainBlock { get { return new WebApi("/chain/StoreNewChainBlock"); } }


        public static WebApi LoadBlockChain { get { return new WebApi("/chain/GetBlockChain"); } }
        public static WebApi GetLatestBlock { get { return new WebApi("/chain/GetLatestBlock"); } }
        public static WebApi LoadBlockBytesFromDisk { get { return new WebApi("/chain/GetBlockBytesFromDisk"); } }
        public static WebApi GetBlocksList { get { return new WebApi("/chain/GetBlocksList"); } }

        public static WebApi Bootstrap { get { return new WebApi("/chain/BootstrapReset"); } }
        public static WebApi CreateNodeChain { get { return new WebApi("/chain/CreateNodeChain"); } }
        public static WebApi StoreBlockBytesToDisk { get { return new WebApi("/chain/StoreBlockBytesToDisk"); } }

        public static WebApi StoreChainToDisk { get { return new WebApi("/chain/StoreChainToDisk"); } }
    }
}
