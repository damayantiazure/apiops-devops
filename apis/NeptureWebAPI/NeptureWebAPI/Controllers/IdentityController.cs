

using Microsoft.AspNetCore.Mvc;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Payloads;
using System.Text.Json;
using System.Text.Json.Serialization;
using static NeptureWebAPI.Controllers.RepositoryController;

namespace NeptureWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly Client client;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(
            Client client,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<IdentityController> logger)
        {
            this.client = client;
            this.jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }



        [HttpPost("search")]
        public async ValueTask<IEnumerable<AzDoIdentity>> SearchIdentitiesAsync([FromBody] IdentitySearchPayload payload)
        {
            return await client.SearchIdentityAsync(payload);
        }
                
        [HttpPost("materialize")]
        public async ValueTask<AzDoIdentity> MaterializeGroupAsync([FromBody] AzDoIdentity payload)
        {
            return await client.MaterializeGroupAsync(payload);
        }

        [HttpPost("translateDescriptors")]
        public async ValueTask<IReadOnlyList<AzDoTranslatedIdentityDescriptor>> TranslateDescriptorsAsync([FromBody] TranslateDescriptorPayload payload)
        {
            return await client.TranslateDescriptorsAsync(payload.SubjectDescriptors);
        }
    }


    public record IdentitySearchPayloadOptions(
        [property: JsonPropertyName("MinResults")] int MinResults,
        [property: JsonPropertyName("MaxResults")] int MaxResults
    );

    public record IdentitySearchPayload(
        [property: JsonPropertyName("query")] string Query,
        [property: JsonPropertyName("identityTypes")] IReadOnlyList<string> IdentityTypes,
        [property: JsonPropertyName("operationScopes")] IReadOnlyList<string> OperationScopes,
        [property: JsonPropertyName("options")] IdentitySearchPayloadOptions Options,
        [property: JsonPropertyName("properties")] IReadOnlyList<string> Properties
    );
    public record TranslateDescriptorPayload(
        [property: JsonPropertyName("subjectDescriptors")] string SubjectDescriptors
    );
}
