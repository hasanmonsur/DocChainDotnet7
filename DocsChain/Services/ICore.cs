using DocChainWeb.ModelsChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.Services
{
    public interface ICore
    {
        //Task<bool> Bootstrap();
        Task<bool> Bootstrap(bool ResetChain);
        Task<bool> BootstrapNodes(IEnumerable<DataBlock> chainBlocks);
        Task<bool> CheckBlockIntegrity(DataBlock newBlock, DataBlock previousBlock);
        Task<bool> CheckFullChainIntegrity();
        Task<NetworkNode> GetMyChainCredentials();
        NetworkNode GetMyChainCredentialSync();
        
        //Task<bool> CreateBlockChain(string ChainFileName);
        Task<DataBlock> CreateNextBlock(byte[] dataToStore, string description);
        Task<DataBlock> StoreReceivedBlock(DataBlock newBlock, byte[] blockData);

        Task<List<DataBlock>> GetBlocksList();
        Task<List<DataBlock>> GetBlocksList(DateTime FromDate);
        Task<List<DataBlock>> GetBlocksList(DateTime FromDate, DateTime ToDate);
        Task<Byte[]> GetDataBlockBytes(int index);
        Task<Byte[]> GetDataBlockBytes(Guid Guid);
        Task<DataBlock> GetLatestBlock();
        Task<BlockChain> LoadBlockChain(string ChainFileName);

        Task<byte[]> LoadBlockBytesFromDisk(Guid FileGuid);
        Task<bool> StoreBlockBytesToDisk(Guid FileGuid, byte[] dataToStore);
       // Task<BlockChain> LoadBlockChain(Guid FileGuid);
        Task<BlockChain> GetFullChain();

        Task<bool> AddTestDocuments(int DocsCount);
        Task<BlockChain> LoadBlockChain();
        Task<bool> AddNode(NetworkNode receivedNode);
        bool StoreChainToDisk(BlockChain myChain);
        Task<BlockChain> BootstrapReset(bool resetChain);
    }
}
