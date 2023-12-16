using DocChainWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.Services
{
    public interface ICore
    {
        Task<bool> Bootstrap();
        Task<bool> Bootstrap(bool ResetChain);
        //Task<BlockChain> BootstrapReset(bool ResetChain);
        Task<bool> BootstrapNodes(IEnumerable<DataBlock> chainBlocks);
        Task<bool> CheckBlockIntegrity(DataBlock newBlock, DataBlock previousBlock);
        Task<bool> CheckFullChainIntegrity();
        Task<NetworkNode> GetMyChainCredentials();
        //Task<BlockChain> CreateBlockChain(string ChainFileName);
        Task<DataBlock> CreateNextBlock(byte[] dataToStore, string description);
        //DataBlock StoreReceivedBlock(DataBlock newBlock, byte[] blockData);

        Task<List<DataBlock>> GetBlocksList();
        Task<List<DataBlock>> GetBlocksList(DateTime FromDate);
        Task<List<DataBlock>> GetBlocksList(DateTime FromDate, DateTime ToDate);
        Task<Byte[]> GetDataBlockBytes(int index);
        Task<Byte[]> GetDataBlockBytes(Guid Guid);
        Task<DataBlock> GetLatestBlock();
        //Task<BlockChain> LoadBlockChain(string ChainFileName);

        Task<BlockChain> GetFullChain();

        Task<bool> AddTestDocuments(int DocsCount);
        Task<List<NetworkNode>> GetAllNetworkNodes();
        Task<bool> StoreNewBlockToChain(DataBlock newBlock);
        Task<IEnumerable<NetworkNode>> CallWelcomeNode(string bootUrl, NetworkNode myNode);
        Task<bool> AddNode(NetworkNode node);
        Task<IEnumerable<DataBlock>> CallGetNodesList(NetworkNode bootNode);
        Task<object> CallGetDataBlockBytes(int index, NetworkNode bootNode);
        NetworkNode GetChainCredentials();
    }
}
