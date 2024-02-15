using System.Text.Json.Serialization;

namespace NeptureWebAPI.AzureDevOps.Payloads
{

    public record AzDoTeamCollection(
        [property: JsonPropertyName("value")] IReadOnlyList<AzDoTeam> Value,
        [property: JsonPropertyName("count")] int Count
    );

    public record AzDoTeam(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("identityUrl")] string IdentityUrl
    );


    public record AzDoAuthNZUser(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("descriptor")] string Descriptor,
        [property: JsonPropertyName("subjectDescriptor")] string SubjectDescriptor,
        [property: JsonPropertyName("providerDisplayName")] string ProviderDisplayName,
        [property: JsonPropertyName("isActive")] bool IsActive,
        [property: JsonPropertyName("resourceVersion")] int ResourceVersion,
        [property: JsonPropertyName("metaTypeId")] int MetaTypeId
    );

    public record AzDoConnectionData(
        [property: JsonPropertyName("authenticatedUser")] AzDoAuthNZUser AuthenticatedUser,
        [property: JsonPropertyName("authorizedUser")] AzDoAuthNZUser AuthorizedUser,
        [property: JsonPropertyName("instanceId")] string InstanceId,
        [property: JsonPropertyName("deploymentId")] string DeploymentId,
        [property: JsonPropertyName("deploymentType")] string DeploymentType
    );



    public record AzDoGroupMembershipSlimCollection(
        [property: JsonPropertyName("count")] int Count,
        [property: JsonPropertyName("value")] IReadOnlyList<AzDoGroupMembershipSlim> Value
    );

    public record AzDoGroupMembershipSlim(
        [property: JsonPropertyName("containerDescriptor")] string ContainerDescriptor,
        [property: JsonPropertyName("memberDescriptor")] string MemberDescriptor
    );


}
