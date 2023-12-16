using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocChainWeb.ModelsChain;
using DocChainWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocChainWeb.Controllers
{

    [Route("[controller]")]
    public class ChainController : Controller
    {
        private readonly ILogger<ChainController> _logger;
        private readonly ICore _chainService;
        private readonly INetworkManager _networkManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;

        

        public ChainController(
            ILogger<ChainController> logger, 
            ICore core, 
            INetworkManager networkManager, 
            IConfiguration configuration,
            IHttpContextAccessor accessor
            )
        {
            _logger = logger;
            _chainService = core;
            _networkManager = networkManager;
            _configuration = configuration;
            _accessor = accessor;
        }


        
        [HttpPost("BootstrapReset")]
        public async Task<BlockChain> BootstrapReset([FromBody] bool ResetChain)
        {
            
            var blockChain=await _chainService.BootstrapReset(ResetChain);

            return blockChain;

        }


        [HttpPost("CreateNodeChain")]
        public async Task<bool> CreateNodeChain([FromBody] NetworkNode receivedNode)
        {                       
            
            //Broadcast my new list to known nodes except the one that just called me
            await _chainService.AddNode(receivedNode);            
            await _chainService.CheckFullChainIntegrity();

            await _networkManager.AddNode(receivedNode);
            await _networkManager.CallNetworkNodesUpdate();

            //return my additional Info
            return true;

        }

        /// <summary>
        /// Receive a new node and send back my description as welcome.
        /// Add Node to the list of known nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [HttpPost("WelcomeNode")]
        public async Task<IEnumerable<NetworkNode>> WelcomeNode([FromBody]NetworkNode receivedNode)
        {
            var nodesList = await _networkManager.GetAllNetworkNodes();
            //Add received node to my personal peer list
            await _networkManager.AddNode(receivedNode);
            //Broadcast my new list to known nodes except the one that just called me
            await _networkManager.CallNetworkNodesUpdate();
            //return my additional Info
            return nodesList;

        }

        [HttpPost("NetworkNodesUpdate")]
        public async Task<bool> NetworkNodesUpdate([FromBody] IEnumerable<NetworkNode> networkNodes)
        {
            var connection = _accessor.HttpContext.Connection;
            _logger.LogWarning($"Received network nodes update from {connection.RemoteIpAddress}:{connection.RemotePort}");            
            foreach (var node in networkNodes)
            {
                await _networkManager.AddNode(node);
            }
            return true;
        }

        // GET api/values
        [HttpGet("GetNodesList")]
        public async Task<IEnumerable<DataBlock>> GetNodesList()
        {
            var blockList = await _chainService.GetBlocksList();

            return blockList;
        }

        // GET api/values
        [HttpPost("AddChainBlock")]
        public async Task<bool> AddChainBlock([FromBody] DataBlock block)
        {
            _logger.LogInformation("Received new Data Block");
            //pick another node for download       
            if (block.Index == 0) throw new Exception("Block Index 0 is invalid");

            var bytes = await _networkManager.GetDataBlockFromRandomNode(block.Index);
            //Add Block to Chain
            await _chainService.StoreReceivedBlock(block,bytes);
            //Verify Chain Integrity
            await _chainService.CheckFullChainIntegrity();   
            
            return true;
        }


        [HttpPost("StoreNewChainBlock")]
        public async Task<bool> StoreNewChainBlock([FromBody] DataBlock block)
        {
            _logger.LogInformation("Received new Data Block");
            //pick another node for download       
            if (block.Index == 0) throw new Exception("Block Index 0 is invalid");

            var bytes = await _chainService.GetDataBlockBytes(block.Guid);
            //Add Block to Chain
            await _chainService.StoreReceivedBlock(block, bytes);
            //Verify Chain Integrity
            await _chainService.CheckFullChainIntegrity();

            await _networkManager.BroadcastNewBlock(block);

            return true;
        }

        [HttpGet("GetDataBlockBytes/{Id}")]
        public async Task<Byte[]> GetDataBlockBytes(int Id)
        {
            return await _chainService.GetDataBlockBytes(Id); ;
        }

        [HttpGet("GetDataBlockList")]
        public async Task<IEnumerable<DataBlock>> GetDataBlockList()
        {
            IEnumerable<DataBlock> chainBlocks = await _chainService.GetBlocksList();

            return chainBlocks;
        }


        [HttpGet("GetBlockChain")]
        public async Task<BlockChain> GetBlockChain()
        {
            BlockChain chainBlocks = await _chainService.LoadBlockChain();

            return chainBlocks;
        }

        [HttpGet("GetLatestBlock")]
        public async Task<DataBlock> GetLatestBlock()
        {
            DataBlock chainBlocks = await _chainService.GetLatestBlock();

            return chainBlocks;
        }

        [HttpPost("GetBlockBytesFromDisk")]
        public async Task<byte[]> GetBlockBytesFromDisk([FromBody] Guid FileGuid)
        {
            var chainBlocksByte = await _chainService.LoadBlockBytesFromDisk(FileGuid);

            //var chainBlocks= Convert.ToBase64String(chainBlocksByte);

            return chainBlocksByte;
        }

        [HttpGet("GetBlocksList")]
        public async Task<IEnumerable<DataBlock>> GetBlocksList()
        {
            IEnumerable<DataBlock> chainBlocks = await _chainService.GetBlocksList();

            return chainBlocks;
        }


        [HttpPost("StoreBlockBytesToDisk")]
        public async Task<bool> StoreBlockBytesToDisk([FromBody] FileData fileData)
        {
            byte[] dataToStore= fileData.DataByte;

            var chainBlocks = await _chainService.StoreBlockBytesToDisk(fileData.Guid, dataToStore);

            return chainBlocks;
        }

        [HttpPost("StoreChainToDisk")]
        public bool StoreChainToDisk([FromBody] BlockChain MyChain)
        {
            bool opStat = false;
            if (MyChain != null)
             opStat = _chainService.StoreChainToDisk(MyChain);
            
            return opStat;
        }
    }
}