using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DocChainWeb.Services;
using DocChainWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocChainWeb.Controllers
{
    
    public class WebAppController : Controller
    {
        private readonly ILogger<WebAppController> _logger;
        private readonly ICore _chainService;
        
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;

        public WebAppController(
           ILogger<WebAppController> logger,
           ICore core,
           INetworkManager networkManager,
           IConfiguration configuration,
           IHttpContextAccessor accessor
           )
        {
            _logger = logger;
            _chainService = core;
            _configuration = configuration;
            _accessor = accessor;
        }

        public async Task<IActionResult> Index()
        {
            //Retrieve current BlockChain
            var  blockList =await _chainService.GetBlocksList();
            if(blockList.Count <=0 )
            {
                blockList = new List<DataBlock>();
            }

            return View(blockList);
        }

        public async Task<IActionResult> System()
        {
            return View();
        }

        public async Task<IActionResult> NodeStatus()
        {
            NodeStatusViewModel model = new NodeStatusViewModel();
            model.Nodes =await _chainService.GetAllNetworkNodes();   
            model.Chain =await _chainService.GetFullChain();

            return View(model);
        }

        public async Task<IActionResult> CreateTestChain()
        {
            //_chainService.AddTestDocuments(3);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ResetChain()
        {
            await _chainService.Bootstrap(true);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BootFromUrl(string bootUrl) {

            bootUrl = $"http://{bootUrl}/";
            if (!String.IsNullOrWhiteSpace(bootUrl))
            {
                var myNode =await _chainService.GetMyChainCredentials();
                //Fill my node from configuration
                myNode.Endpoint =_configuration["NodeIdentification:IPEndpoint"];

                IEnumerable<NetworkNode> remoteNodes =await _chainService.CallWelcomeNode(bootUrl, myNode);

                //IEnumerable<NetworkNode> remoteNodes = _networkManager.CallWelcomeNode(bootUrl, myNode).Result;
                //Add remote credentials to my list
                foreach (var node in remoteNodes)
                {
                    await _chainService.AddNode(node);

                    //_networkManager.AddNode(node);
                }
                NetworkNode bootNode = new NetworkNode()
                {
                    AccessKey = "",
                    Description = "",
                    Endpoint = bootUrl
                };

                //Get Chain
                IEnumerable<DataBlock> chainBlocks =await _chainService.CallGetNodesList(bootNode);

                //IEnumerable<DataBlock> chainBlocks = _networkManager.CallGetNodesList(bootNode).Result;
                //Replace my Chain Nodes
                await _chainService.BootstrapNodes(chainBlocks);
                _logger.LogInformation($"Rebuilding local blockchain");
                //Download Chain to local folder
                foreach (var block in chainBlocks)
                {
                    _logger.LogInformation($"Downloading DocsChain Desc {block.Description}");
                    _logger.LogInformation($"Downloading DocsChain Node {block.Index}");
                    //Download Data from the remote server
                    //var bytesToStore =await _chainService.CallGetDataBlockBytes(block.Index, bootNode);

                    //var bytesToStore = _networkManager.CallGetDataBlockBytes(block.Index, bootNode).Result;

                    //_logger.LogInformation($"{bytesToStore.Length / 1024} KBytes downloaded");

                    //_chainService.StoreBlockBytesToDisk(block.Guid, bytesToStore);
                }
                //Validate The chain
                _logger.LogInformation("Validating downloaded DocsChain...");

                await _chainService.CheckFullChainIntegrity();
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            var fileName = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));
            
            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(memoryStream);
            var bytes = binaryReader.ReadBytes((int)memoryStream.Length);

            var newBlock =await _chainService.CreateNextBlock(bytes, fileName);

            await _chainService.StoreNewBlockToChain(newBlock);
            
            return RedirectToAction("Index");
        }

        private async Task<string> GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpGet]
        public async Task<IActionResult> Download(int Id)
        {
            var blockList = await _chainService.GetFullChain();
            
            var block=blockList.NodesList.Where(x => x.Index == Id).First();
            var nodeBytes = await _chainService.GetDataBlockBytes(block.Guid);

            return File(nodeBytes, await GetContentType(block.Description), block.Description);
        }

        [HttpGet]
        public async Task<string> GetNodeAsBase64(int Id)
        {
            var nodeBytes =await _chainService.GetDataBlockBytes(Id);
            //var sd= Convert.ToBase64String(nodeBytes);
            return Convert.ToBase64String(nodeBytes);
        }
    }
}