using DocChainWeb.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DocChainWeb.Services
{
    //WebApi objWebApi = new WebApi();

    public class NetworkManager : INetworkManager
    {
        private readonly ILogger<NetworkManager> _logger;
        private readonly IConfiguration _configuration;

        private List<NetworkNode> NetworkNodes;
        private NetworkNode myNode;

        public NetworkManager(
            ILogger<NetworkManager> logger,
            IConfiguration Configuration
            )
        {
            _logger = logger;
            _configuration = Configuration;
            _logger.LogWarning("Initiating Network Manager");

            LoadNodes();
        }

        
        public async Task<BlockChain> Bootstrap(bool ResetNode)
        {
            myNode = NetworkNodes.ToList().First();

            HttpClient client = new HttpClient();
            BlockChain blockChain = new BlockChain();

            client.BaseAddress = new Uri(myNode.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.PostAsync(WebApi.Bootstrap.Value, new StringContent(JsonConvert.SerializeObject(ResetNode), System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blockChain = JsonConvert.DeserializeObject<BlockChain>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }

            return blockChain;
        }

        public async Task<bool> StoreChainToDisk(NetworkNode myNode, BlockChain myChain)
        {
            myNode = NetworkNodes.ToList().First();

            HttpClient client = new HttpClient();
            bool opState = false;
            client.BaseAddress = new Uri(myNode.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.PostAsync(WebApi.StoreChainToDisk.Value, new StringContent(JsonConvert.SerializeObject(myChain), System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    opState = JsonConvert.DeserializeObject<bool>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }

            return opState;
        }

        

        public async Task<bool> CreateNodeChain(string NodesFileName)
        {
            try
            {
                myNode = NetworkNodes.ToList().First();

                _logger.LogInformation($"Node Dump to Disk requested...");
                var node=new NetworkNode();
                node.Description = "backup-node";
                node.Endpoint = "http://192.168.0.101:7557";
                node.AccessKey = null;
                NetworkNodes.Add(node);

                HttpClient client = new HttpClient();
                BlockChain blockChain = new BlockChain();
                client.BaseAddress = new Uri(myNode.Endpoint); //"http://localhost:64195/"
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpResponseMessage response = await client.PostAsync(WebApi.CreateNodeChain.Value, new StringContent(JsonConvert.SerializeObject(NetworkNodes), System.Text.Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        //Transform JSON to object
                        blockChain = JsonConvert.DeserializeObject<BlockChain>(responseString);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Cannot call remote Node");
                }


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Utilities
        public async Task<BlockChain> LoadBlockChain(NetworkNode node)
        {
            HttpClient client = new HttpClient();
            BlockChain blockChain =new BlockChain();
            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(WebApi.LoadBlockChain.Value);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blockChain = JsonConvert.DeserializeObject<BlockChain>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }

            return blockChain;

        }

        public async Task<DataBlock> GetLatestBlock(NetworkNode node)
        {
            HttpClient client = new HttpClient();
            DataBlock blockChain = new DataBlock();

            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(WebApi.GetLatestBlock.Value);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blockChain = JsonConvert.DeserializeObject<DataBlock>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }

            return blockChain;
        }


        public async Task<byte[]> LoadBlockBytesFromDisk(NetworkNode node, Guid FileGuid)
        {
            HttpClient client = new HttpClient();
            byte[] blockByte = null;

            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.PostAsync(WebApi.LoadBlockBytesFromDisk.Value, new StringContent(JsonConvert.SerializeObject(FileGuid), System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blockByte = JsonConvert.DeserializeObject<byte[]>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }

            //var blockByte=Encoding.UTF8.GetBytes(dataString);

            return blockByte;

        }


        public async Task<bool> StoreBlockBytesToDisk(NetworkNode node,Guid guid, byte[] dataToStore)
        {
            bool opStat = false;
            HttpClient client = new HttpClient();
            var fileData = new FileData();
            fileData.Guid = guid;
            fileData.DataByte = dataToStore;// Encoding.UTF8.GetString(dataToStore);

            //byte[] dataToStore1 = Encoding.UTF8.GetBytes(fileData.Base64String);


            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                //var ser = JsonConvert.SerializeObject(fileData);

                HttpResponseMessage response = await client.PostAsync(WebApi.StoreBlockBytesToDisk.Value, new StringContent(JsonConvert.SerializeObject(fileData), System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    opStat = JsonConvert.DeserializeObject<bool>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }
                        

            return opStat;

        }



        public async Task<List<DataBlock>> GetBlocksList(NetworkNode node)
        {
            HttpClient client = new HttpClient();
            List<DataBlock> blockList = new List<DataBlock>();

            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(WebApi.GetBlocksList.Value);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blockList = JsonConvert.DeserializeObject<List<DataBlock>>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }

            return blockList.ToList();
        }

        public async Task<NetworkNode> GetNodeByUrl(string url) {

            return NetworkNodes.First(x=>x.Endpoint==url);
        }

        public async Task<IEnumerable<NetworkNode>> GetAllNetworkNodes()
        {
            return NetworkNodes;
        }

        public async Task<bool> LoadNodes()
        {
                 _logger.LogWarning("No nodes founds on file, adding myself to nodes");
                NetworkNode myselfNode = new NetworkNode();

                myselfNode.Description = _configuration["NodeIdentification:Name"];
                myselfNode.Endpoint = _configuration["NodeIdentification:IPEndpoint"];
                await AddNode(myselfNode);


            return true;
        }
                
        //Node is added only when a response from a remote party is detected
        public async Task<bool> AddNode(NetworkNode networkNode)
        {
            _logger.LogInformation($"Request to Add {networkNode.Endpoint}");
            if (NetworkNodes == null) NetworkNodes = new List<NetworkNode>();

            //Don't add myself to the list or I'll loop, skip existing items
            if (NetworkNodes.FirstOrDefault(x => x.Endpoint == networkNode.Endpoint) == null)
            {
                _logger.LogInformation($"adding New Node {networkNode.Endpoint}:{networkNode.AccessKey}");
                NetworkNodes.Add(networkNode);
                //this.DumpNodesToDisk();
            }
            else
            {
                _logger.LogWarning("Node yet present");
            }


            return true;

        }

               

        #endregion

        #region RemoteCalls

        public async Task<IEnumerable<NetworkNode>> CallWelcomeNode(string remoteUrl, NetworkNode node)
        {
            HttpClient client = new HttpClient();
            IEnumerable<NetworkNode> remoteNode = null;
            client.BaseAddress = new Uri(remoteUrl); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.PostAsync(WebApi.WelcomeNode.Value, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(node), System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    remoteNode = JsonConvert.DeserializeObject<IEnumerable<NetworkNode>>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }
            return remoteNode;

        }

        public async Task<bool> StoreBlockToChain(string remoteUrl,DataBlock block) {
            HttpClient client = new HttpClient();
            bool? remoteResponse = null;
            client.BaseAddress = new Uri(remoteUrl); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.PostAsync(WebApi.StoreNewChainBlock.Value, new StringContent(JsonConvert.SerializeObject(block), System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    remoteResponse = JsonConvert.DeserializeObject<Boolean>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }
            

            return Convert.ToBoolean(remoteResponse);
        }

        public async Task<IEnumerable<DataBlock>> CallGetNodesList(NetworkNode node)
        {
            HttpClient client = new HttpClient();
            IEnumerable<DataBlock> blocks = null;
            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(WebApi.GetNodesList.Value);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blocks = JsonConvert.DeserializeObject<IEnumerable<DataBlock>>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }
            return blocks;

        }

        public async Task<bool> StoreNewBlockToChain(DataBlock newBlock) {
            foreach (var node in NetworkNodes)
            {
                await StoreBlockToChain(node.Endpoint, newBlock);
            }

            return true;
        }


        public async Task<byte[]> GetDataBlockFromRandomNode(int Id) {
            Random rnd = new Random();
            var nodesWithoutMe = NetworkNodes.Where(x => x.Endpoint != _configuration["NodeIdentification:IPEndpoint"]);
                     
            while (true) {
                var randomIndex = rnd.Next(nodesWithoutMe.Count());
                var randomNetworkNode = nodesWithoutMe.OrderBy(item => rnd.Next()).ToList().First();
                var bytes = await CallGetDataBlockBytes(Id, randomNetworkNode);
                if (bytes.Length > 0) {
                    return bytes;
                }
                Thread.Sleep(1000);
            }
        }

        public async Task<byte[]> CallGetDataBlockBytes(int Id, NetworkNode node)
        {
            HttpClient client = new HttpClient();
            byte[] blockBytes = null;
            client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var url = WebApi.GetDataBlockBytes.Value + $"{Id}";
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //Transform JSON to object
                    blockBytes = JsonConvert.DeserializeObject<byte[]>(responseString);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot call remote Node");
            }
            return blockBytes;
        }

        public async Task<bool> CallNetworkNodesUpdate()
        {
            HttpClient client = new HttpClient();
            foreach (var node in NetworkNodes.Where(x=>x.Endpoint != _configuration["NodeIdentification:IPEndpoint"]))
            {
                _logger.LogInformation($"Sending node list update to {node.Endpoint}");
                client.BaseAddress = new Uri(node.Endpoint); //"http://localhost:64195/"
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpResponseMessage response = await client.PostAsync(WebApi.NetworkUpdate.Value, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(NetworkNodes), System.Text.Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        //Transform JSON to object
                        var callResponse = JsonConvert.DeserializeObject<bool>(responseString);
                        _logger.LogInformation($"Node answer is Received={callResponse}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, $"Cannot call remote Node {node.Endpoint}");
                }

            }
            return true;
        }
               

        #endregion


    }
}
