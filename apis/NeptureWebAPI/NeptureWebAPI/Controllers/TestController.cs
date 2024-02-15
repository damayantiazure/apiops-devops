using Microsoft.AspNetCore.Mvc;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Payloads;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NeptureWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly Client client;
        private readonly ILogger<TestController> _logger;

        public TestController(
            Client client,
            ILogger<TestController> logger)
        {
            this.client = client;
            _logger = logger;
        }

        [HttpGet("loopback")]
        public async Task<AzDoConnectionData> GetConnectionDataAsync()
        {
            return await client.GetConnectionDataAsync(true);
        }

        [HttpGet("projects")]
        public async Task<AzDoConnectionData> GetProjectsAsync()
        {
            return await client.GetProjectsAsync(true);
        }
    }
}
