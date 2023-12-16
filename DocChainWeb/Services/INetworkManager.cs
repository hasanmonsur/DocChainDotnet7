using DocChainWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.Services
{
    public interface INetworkManager
    {
        Task<BlockChain> Bootstrap(bool ResetNode);
        Task<bool> LoadNodes();
        Task<bool> AddNode(NetworkNode networkNode);
        Task<NetworkNode> GetNodeByUrl(string url);
        Task<IEnumerable<NetworkNode>> GetAllNetworkNodes();
        Task<IEnumerable<NetworkNode>> CallWelcomeNode(string remoteUrl, NetworkNode node);
        Task<IEnumerable<DataBlock>> CallGetNodesList(NetworkNode node);
        Task<byte[]> CallGetDataBlockBytes(int Id, NetworkNode node);
        Task<bool> CallNetworkNodesUpdate();
        Task<byte[]> GetDataBlockFromRandomNode(int Id);
        Task<bool> StoreNewBlockToChain(DataBlock block);
        Task<List<DataBlock>> GetBlocksList(NetworkNode node);
        Task<BlockChain> LoadBlockChain(NetworkNode node);
        Task<DataBlock> GetLatestBlock(NetworkNode myNode);
        Task<byte[]> LoadBlockBytesFromDisk(NetworkNode myNode, Guid fileGuid);
        Task<bool> StoreBlockBytesToDisk(NetworkNode node,Guid guid, byte[] dataToStore);
        Task<bool> StoreChainToDisk(NetworkNode myNode, BlockChain myChain);
    }
}
