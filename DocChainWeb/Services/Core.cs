using DocChainWeb.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DocChainWeb.Services
{
    public class Core : ICore
    {
        private readonly ILogger<Core> _logger;
        private readonly IConfiguration _configuration;
        private readonly INetworkManager _networkManager;

        private BlockChain MyChain = new BlockChain();
        private NetworkNode MyNode =new NetworkNode();



        public Core(ILogger<Core> logger,
            IConfiguration Configuration,
            INetworkManager networkManager
            )
        {
            _configuration = Configuration;
            _logger = logger;
            _networkManager = networkManager;           

            if (MyChain.NodesList == null)
            {
                Bootstrap();

                CheckFullChainIntegrity();

                MyNode.Endpoint = _configuration["NodeIdentification:IPEndpoint"];
                MyNode.AccessKey = _configuration["NodeIdentification:AccessKey"];
                MyNode.Description = _configuration["NodeIdentification:Name"];
            }
        }

        public async Task<bool> Bootstrap()
        {
            bool opStat = false;

            opStat = await Bootstrap(opStat);

            return opStat;
        }


        public async Task<bool> Bootstrap(bool ResetChain)
        {
            bool opStat = false;

            MyChain = await _networkManager.Bootstrap(ResetChain);

            return opStat;
        }

        public async Task<bool> AddTestDocuments(int DocsCount) {
            byte[] fileToStore = null;
            for (int i = 0; i < DocsCount; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        fileToStore = File.ReadAllBytes("SamplesPDF\\sample1.pdf");
                        break;
                    case 1:
                        fileToStore = File.ReadAllBytes("SamplesPDF\\sample2.pdf");
                        break;
                    case 2:
                        fileToStore = File.ReadAllBytes("SamplesPDF\\sample3.pdf");
                        break;
                }
                CreateNextBlock(fileToStore, $"sample_{i}.pdf");
                _logger.LogTrace($"cycle {i}");
            }

            return true;

        }

        public async Task<BlockChain> GetFullChain() {

            var myChain = await _networkManager.LoadBlockChain(MyNode);

            return myChain;
        }

        public async Task<bool> BootstrapNodes(IEnumerable<DataBlock> chainBlocks)
        {
            MyChain.NodesList = chainBlocks.ToList();
            //this.DumpChainToDisk();
            return true;
        }


        public async Task<bool> LoadBlockChain()
        {
            //Load the chain and validate all blocks
            MyChain = await _networkManager.LoadBlockChain(MyNode);

            return true;
        }

        public async Task<DataBlock> GetLatestBlock()
        {
            var block = await _networkManager.GetLatestBlock(MyNode);
           // return MyChain.NodesList.OrderByDescending(x => x.Timestamp).First();

           return block;
        }

        public async Task<List<DataBlock>> GetBlocksList()
        {
            
            var blockList=await _networkManager.GetBlocksList(MyNode);

            return blockList;
        }

        public async Task<byte[]> LoadBlockBytesFromDisk(Guid FileGuid)
        {
            var blockByte= await _networkManager.LoadBlockBytesFromDisk(MyNode, FileGuid);

            return blockByte;
        }

        public async Task<IEnumerable<NetworkNode>> CallWelcomeNode(string bootUrl, NetworkNode myNode)
        {

            var networkNodeList = await _networkManager.CallWelcomeNode(bootUrl, myNode);

            return networkNodeList;
        }

        public async Task<bool> AddNode(NetworkNode node)
        {
            await _networkManager.AddNode(node);

            return true;
        }

        public async Task<IEnumerable<DataBlock>> CallGetNodesList(NetworkNode bootNode)
        {
            var dataBlockList = await _networkManager.CallGetNodesList(bootNode);

            return dataBlockList;
        }

        public async Task<object> CallGetDataBlockBytes(int index, NetworkNode bootNode)
        {
            var dataBlockList = await _networkManager.CallGetDataBlockBytes(index, bootNode); 

            return dataBlockList;
        }



        public async Task<List<DataBlock>> GetBlocksList(DateTime FromDate)
        {
            List<DataBlock> dataBlockList = new List<DataBlock>();

            return dataBlockList;
        }

        public async Task<List<NetworkNode>> GetAllNetworkNodes()
        {
           var nodelist=await _networkManager.GetAllNetworkNodes();

            return nodelist.ToList();
        }

        public async Task<bool> StoreNewBlockToChain(DataBlock newBlock)
        {
            await _networkManager.StoreNewBlockToChain(newBlock);

            return true;
        }

        public async Task<List<DataBlock>> GetBlocksList(DateTime FromDate, DateTime ToDate)
        {
            List<DataBlock> dataBlockList = new List<DataBlock>();
            return dataBlockList;
        }

        public async Task<DataBlock> CreateNextBlock(byte[] dataToStore, string description)
        {
            _logger.LogInformation("Creating a new Chain Block");
            
            var previousBlock = await GetLatestBlock();
            DataBlock newBlock = new DataBlock();

            newBlock.Index = previousBlock.Index + 1;
            newBlock.Description = description;
            newBlock.PreviousHash = previousBlock.Hash;
            newBlock.Salt =await GenerateSalt();
            newBlock.DataSize = dataToStore.Length;

            //Dump DataBlock to Disk with GUID filename
            await _networkManager.StoreBlockBytesToDisk(MyNode,newBlock.Guid, dataToStore);

            _logger.LogInformation($"Block Index {newBlock.Index}");
            _logger.LogInformation($"Block Description {newBlock.Description}");
            _logger.LogInformation($"Block Data To Store Size {dataToStore.Length / 1024} KBytes");
            _logger.LogInformation($"Calculating new Hash");

            newBlock.Hash = await CalculateBlockHash(
                    newBlock.Index,
                    newBlock.Description,
                    newBlock.PreviousHash,
                    newBlock.Timestamp,
                    newBlock.Salt,
                    newBlock.Guid);

            _logger.LogInformation($"Hash Calculated {newBlock.Hash}");

            //MyChain=await _networkManager.LoadBlockChain(MyNode);
            //MyChain.NodesList.Add(newBlock);

            _logger.LogInformation($"Block Added, stored on disk at {newBlock.Guid}");

            await _networkManager.StoreChainToDisk(MyNode, MyChain);

            return newBlock;
        }

        public async Task<Byte[]> GetDataBlockBytes(int index)
        {
            var blockList = await _networkManager.GetBlocksList(MyNode);

            var node = blockList.First(x => x.Index == index);
            var bytes = await LoadBlockBytesFromDisk(node.Guid);

            return bytes;
        }

        public async Task<Byte[]> GetDataBlockBytes(Guid Guid)
        {
            var bytes = await LoadBlockBytesFromDisk(Guid);

            return bytes;
        }

        public async Task<NetworkNode> GetMyChainCredentials()
        {
            NetworkNode node = new NetworkNode();
            node.Description =  _configuration["NodeIdentification:Name"];
            node.AccessKey = MyChain.UniqueId.ToString();

            return node;
        }

        public NetworkNode GetChainCredentials()
        {
            NetworkNode node = new NetworkNode();
            node.Description = _configuration["NodeIdentification:Name"];
            node.AccessKey = MyChain.UniqueId.ToString();

            return node;
        }

        public async Task<bool> CreateBlockChain(string ChainFileName)
        {
            try
            {
                //Create the Chain with genesis block
                MyChain.ChainDescription = "DocsChain 0.1 POC by Giulio Fronterotta";
                MyChain.GenesisDate = DateTime.Now;
                MyChain.UniqueId = Guid.NewGuid();
                MyChain.NodesList = new List<DataBlock>();

                //Now create Genesis Node


                DataBlock GenesisBlock = new DataBlock();
                GenesisBlock.Index = 0;
                GenesisBlock.PreviousHash = "0";
                GenesisBlock.Timestamp = DateTime.Now;
                GenesisBlock.Salt = await GenerateSalt();
                GenesisBlock.Description = "Genesis Block";

                //StoreBlockBytesToDisk(GenesisBlock.Guid, Encoding.ASCII.GetBytes(GENESIS_BLOCK_DATA));

                GenesisBlock.Hash = await CalculateBlockHash(
                    GenesisBlock.Index,
                    GenesisBlock.Description,
                    GenesisBlock.PreviousHash,
                    GenesisBlock.Timestamp,
                    GenesisBlock.Salt,
                    GenesisBlock.Guid);

                MyChain.NodesList.Add(GenesisBlock);

                //this.DumpChainToDisk();
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }

        private async Task<string> GenerateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        private async Task<string> CalculateBlockHash(DataBlock block)
        {
            return  await CalculateBlockHash(block.Index, block.Description, block.PreviousHash, block.Timestamp, block.Salt, block.Guid);
        }

        private async Task<string> CalculateBlockHash(int index, string description, string previousHash, DateTime timestamp, string salt, Guid Guid)
        {
            _logger.LogTrace("Calculating Block Hash");

            byte[] indexBytes = Encoding.ASCII.GetBytes(index.ToString("D20"));
            byte[] descriptionBytes = Encoding.ASCII.GetBytes(description);
            byte[] previousHashBytes = Encoding.ASCII.GetBytes(previousHash);
            byte[] timestampBytes = Encoding.ASCII.GetBytes(timestamp.ToString("yyyyMMddHHmmssf"));
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            byte[] data = await LoadBlockBytesFromDisk(Guid);


            byte[] theBlock = new byte[indexBytes.Length + descriptionBytes.Length + previousHashBytes.Length + timestampBytes.Length + saltBytes.Length + data.Length];
            _logger.LogTrace("Copying bytes for Hashing");
            System.Buffer.BlockCopy(indexBytes, 0, theBlock, 0, indexBytes.Length);
            System.Buffer.BlockCopy(descriptionBytes, 0, theBlock, indexBytes.Length, descriptionBytes.Length);
            System.Buffer.BlockCopy(previousHashBytes, 0, theBlock, indexBytes.Length + descriptionBytes.Length, previousHashBytes.Length);
            System.Buffer.BlockCopy(timestampBytes, 0, theBlock, indexBytes.Length + descriptionBytes.Length + previousHashBytes.Length, timestampBytes.Length);
            System.Buffer.BlockCopy(saltBytes, 0, theBlock, indexBytes.Length + descriptionBytes.Length + previousHashBytes.Length + timestampBytes.Length, saltBytes.Length);
            System.Buffer.BlockCopy(data, 0, theBlock, indexBytes.Length + descriptionBytes.Length + previousHashBytes.Length + timestampBytes.Length + saltBytes.Length, data.Length);
            _logger.LogTrace($"Copying bytes terminated - Calculate Hash for block {index} - desc {description} - salt {salt}");

            //Calculate Hash
            using (var sha512 = SHA512.Create())
            {
                // Send a sample text to hash.
                var hashedBytes = sha512.ComputeHash(theBlock);

                // Get the hashed string.
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                _logger.LogTrace($"Magic SHA512 Hash is -> [{hash}]");
                // Print the string. 
                return hash;
            }
        }

        public async Task<bool> CheckBlockIntegrity(DataBlock newBlock, DataBlock previousBlock)
        {
            _logger.LogInformation("Blocks integrity check initiated");

            if (previousBlock.Index + 1 != newBlock.Index)
            {
                _logger.LogError($"new Block received has invalid index. Expected {previousBlock.Index + 1}, received {newBlock.Index}");
                return false;
            }
            else if (previousBlock.Hash != newBlock.PreviousHash)
            {
                _logger.LogError($"new Block 'previous hash' is different from the latest block 'previous hash' field. \r\nExpected {previousBlock.PreviousHash}\r\n received {newBlock.PreviousHash}");
                return false;
            }
            else if (await CalculateBlockHash(newBlock) != newBlock.Hash)
            {
                _logger.LogError("Recalculated Block Hash is different from the received one.");
                return false;
            }
            _logger.LogInformation($"Integrity check passed  for block {newBlock.Index}:[{newBlock.Description}] against previous Block {previousBlock.Index}:[{previousBlock.Description}]");
           
            return true;
        }

        public async Task<bool> CheckFullChainIntegrity()
        {
            _logger.LogWarning("Start of Full Chain integrity Check");

            DataBlock previousBlock =new DataBlock();

            foreach (var block in MyChain.NodesList.OrderBy(x => x.Index))
            {
                if (block.Index == 0)
                {
                    previousBlock = block;
                    //Skip Genesis Block
                }
                else
                {
                    //Load node Data for checking                    
                    if (await CheckBlockIntegrity(block, previousBlock))
                    {
                        //Empty block to avoid data write on chain directly                       
                        previousBlock = block;
                        _logger.LogInformation($"Block {block.Index} verified");
                    }
                    else
                    {
                        _logger.LogCritical($"Integrity Broken at node {block.Index}");
                        return false;
                    }
                }
            }
            _logger.LogInformation("Full chain integrity verified");

            return true;
        }
    }
}
