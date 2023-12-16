using DocChainWeb.ModelsChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocChainWeb.Services
{
    public interface INetworkManager
    {
        Task<bool> LoadNodes();
        Task<bool> AddNode(NetworkNode networkNode);
        Task<NetworkNode> GetNodeByUrl(string url);
        Task<IEnumerable<NetworkNode>> GetAllNetworkNodes();
        Task<IEnumerable<NetworkNode>> CallWelcomeNode(string remoteUrl, NetworkNode node);
        Task<IEnumerable<DataBlock>> CallGetNodesList(NetworkNode node);
        Task<byte[]> CallGetDataBlockBytes(int Id, NetworkNode node);
        Task<bool> CallNetworkNodesUpdate();
        Task<byte[]> GetDataBlockFromRandomNode(int Id);
        Task<bool> BroadcastNewBlock(DataBlock newBlock);
    }
}
