
using Microsoft.AspNetCore.Mvc;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Payloads;
using System;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace NeptureWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassificationNodeController : ControllerBase
    {
        private readonly Client client;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<TeamsController> _logger;

        public ClassificationNodeController(
            Client client,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<TeamsController> logger)
        {
            this.client = client;
            this.jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        [HttpPost("new")]
        public async ValueTask<AzDoClassificationNodeCreatedResponse> CreateNewNodeAsync([FromBody] NewNodePayload newNodePayload)
        {
            return await client.CreateNewClassificationNodeAsync(newNodePayload);
        }

        private async ValueTask CreateNodeIfNotExistsAsync(string projectId, string[] paths, int currentIndex, ClassificationNode parentNode, bool isAreaPath)
        {
            if (currentIndex >= paths.Length)
            {
                return;
            }
            
            var activeNodeName = paths[currentIndex];
            var activeNode = default(ClassificationNode);

            if (parentNode.Children != null && parentNode.Children.Count > 0)
            {
                activeNode = parentNode.Children.FirstOrDefault((node) => node.Name.ToLower() == activeNodeName.ToLower());
            }

            if (activeNode != null && activeNode.Identifier != null)
            {
                await CreateNodeIfNotExistsAsync(projectId, paths, currentIndex + 1, activeNode, isAreaPath);
            }
            else
            {
                var response = await client.CreateNewClassificationNodeAsync(new NewNodePayload(projectId, activeNodeName, parentNode.Identifier, isAreaPath));
                if (response != null && response.Success && response.Node != null && !string.IsNullOrWhiteSpace(response.Node.Id))
                {
                    var node = response.Node;
                    var createdNode = new ClassificationNode(0, Identifier: node.Id, node.Text, string.Empty, false, new List<ClassificationNode>(), string.Empty, string.Empty);
                    await CreateNodeIfNotExistsAsync(projectId, paths, currentIndex + 1, createdNode, isAreaPath);
                }
            }
        }

        [HttpPost("ensurePath")]
        public async ValueTask<bool> EnsurePathAsync([FromBody] EnsurePathPayload payload)
        {
            var paths =  payload.path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (paths != null && paths.Length > 0)
            {
                var rootNode = await client.GetClassificationNodeAsync(payload.projectId, payload.isAreaPath);

                await this.CreateNodeIfNotExistsAsync(payload.projectId, paths, 0, rootNode, payload.isAreaPath);

            }
            return true;
        }
    }

    public record NewNodePayload(string projectId, string nodeName, string parentId, bool isAreaPath);
    public record EnsurePathPayload(string projectId, bool isAreaPath, string path);
}
