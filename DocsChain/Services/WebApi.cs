namespace DocsChain.Services
{
    public class WebApi
    {
        private WebApi(string value) { Value = value; }

        public string Value { get; set; }

        public static WebApi GetNodesList { get { return new WebApi("/chain/GetNodesList"); } }
        public static WebApi WelcomeNode { get { return new WebApi("/chain/WelcomeNode"); } }
        public static WebApi NetworkUpdate { get { return new WebApi("/chain/NetworkNodesUpdate"); } }
        public static WebApi GetDataBlockBytes { get { return new WebApi("/chain/GetDataBlockBytes/"); } }
        public static WebApi AddChainBlock { get { return new WebApi("/chain/AddChainBlock"); } }


    }
}
