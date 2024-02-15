
using Microsoft.AspNetCore.Mvc;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Payloads;

namespace NeptureWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly Client client;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(
            Client client,
            ILogger<TeamsController> logger)
        {
            this.client = client;
            _logger = logger;
        }

        [HttpGet("loopback")]
        public async Task<string> Get()
        {
            return await client.GetHealthInfo();
        }

        [HttpGet("all")]
        public async Task<AzDoTeamCollection> GetTeamsAsync([FromQuery] int top = 10, [FromQuery] int skip = 0)
        {
            return await client.GetTeamsAsync(mine: true, top, skip);
        }

        [HttpGet("group-memberships")]
        public async Task<AzDoGroupMembershipSlimCollection> GetGroupMembershipsAsync()
        {
            var connectionData = await client.GetConnectionDataAsync();
            var subjectDescriptor = connectionData.AuthenticatedUser.SubjectDescriptor;

            return await client.GetGroupMembershipsAsync(subjectDescriptor);
        }
    }
}
