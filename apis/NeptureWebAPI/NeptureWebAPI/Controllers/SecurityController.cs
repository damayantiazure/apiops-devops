

using Microsoft.AspNetCore.Mvc;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Payloads;
using System.Text.Json;

namespace NeptureWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly Client client;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(Client client, JsonSerializerOptions jsonSerializerOptions, ILogger<SecurityController> logger)
        {
            this.client = client;
            this.jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        [HttpPost("applyAckls/{namespaceId}")]
        public async ValueTask<bool> ApplyAcksAsync(string namespaceId, [FromBody] AzDoAclEntryCollection[] aces)
        {
            return await client.ApplyAcksAsync(namespaceId, aces);
        }

        [HttpPost("updateRoleAssignment")]
        public async ValueTask<bool> UpdateRoleAssignmentAsync([FromBody] UpdateRoleAssignmentPayload payload)
        {
            return await client.UpdateRoleAssginmentAsync(payload.ApiVersion, payload.ProjectId, payload.ResourceId, payload.Seperator, payload.Scope, payload.Body);
            
        }

        [HttpPost("updateRoleInheritance")]
        public async ValueTask<bool> UpdateRoleInheritanceAsync([FromBody] UpdateRoleInheritancePayload payload)
        {
            return await client.UpdateRoleInheritanceAsync(payload.ApiVersion, payload.ProjectId, payload.ResourceId, payload.Seperator, payload.Scope, payload.inheritPermissions);
        }

        [HttpPost("roleAssignnments")]
        public async Task<AzDoTypeRoleAssignment[]> GetRoleAssignnmentsAsync([FromBody] GetRoleAssignmentPayload payload)
        {
            return await client.GetRoleAssignmentAsync(payload.ApiVersion, payload.ProjectId, payload.ResourceId, payload.Seperator, payload.Scope);
        }

        [HttpPost("deleteRoleAssignnments")]
        public async Task<bool> DeleteRoleAssignnmentsAsync([FromBody] DeleteRoleAssignmentPayload payload)
        {
            return await client.DeleteRoleAssignmentAsync(payload.ApiVersion, payload.ProjectId, payload.ResourceId, payload.Seperator, payload.Scope, payload.Identities);
        }

        public abstract class BaseRolePayload
        {
            public string ApiVersion { get; set; }
            public string ProjectId { get; set; }
            public string ResourceId { get; set; }
            public string Seperator { get; set; }
            public string Scope { get; set; }
        }
        public class GetRoleAssignmentPayload : BaseRolePayload
        {

        }

        public class DeleteRoleAssignmentPayload : BaseRolePayload
        {
            public string[] Identities { get; set; }
        }

        public class UpdateRoleAssignmentPayload : BaseRolePayload
        {

            public AzDoRoleAssignment[] Body { get; set; }            
        }

        public class UpdateRoleInheritancePayload : BaseRolePayload
        {
            public bool inheritPermissions { get; set; }
        }
    }
}
