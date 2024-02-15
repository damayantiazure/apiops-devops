using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Payloads;
using System.Text.Json;
using static NeptureWebAPI.Controllers.SecurityController;

namespace NeptureWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly Client client;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<RepositoryController> _logger;

        public RepositoryController(Client client, JsonSerializerOptions jsonSerializerOptions, ILogger<RepositoryController> logger)
        {
            this.client = client;
            this.jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        [HttpPost("create")]
        public async ValueTask<AzDoRepository> CreateRepositoryAsync([FromBody] CreateRepositoryPayload payload)
        {
            return await client.CreateRepositoryAsync(payload.ProjectId, payload.RepositoryName);

        }

        public class CreateRepositoryPayload
        {
            public string ProjectId { get; set; }
            public string RepositoryName { get; set; }
        }
    }
}
