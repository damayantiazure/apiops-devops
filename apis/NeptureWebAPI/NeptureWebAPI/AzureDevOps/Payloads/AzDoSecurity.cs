

using System.Text.Json.Serialization;

namespace NeptureWebAPI.AzureDevOps.Payloads
{
    public record AzDoAclDictionaryEntry(
        [property: JsonPropertyName("descriptor")] string Descriptor,
        [property: JsonPropertyName("allow")] int Allow,
        [property: JsonPropertyName("deny")] int Deny
    );

    public record AzDoAclEntryCollection(
        [property: JsonPropertyName("inheritPermissions")] bool InheritPermissions,
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("acesDictionary")] Dictionary<string, AzDoAclDictionaryEntry> AcesDictionary
    );

    public record AzDoAclEntryPostBody([property: JsonPropertyName("value")] AzDoAclEntryCollection[] Value);

    public record AzDoRoleAssignment(
        [property: JsonPropertyName("roleName")] string RoleName,
        [property: JsonPropertyName("userId")] string UserId
    );

    public record AzDoTypeIdentity(
        [property: JsonPropertyName("displayName")] string DisplayName,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("uniqueName")] string UniqueName
    );

    public record AzDoRoleDefinition(
        [property: JsonPropertyName("displayName")] string DisplayName,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("allowPermissions")] int AllowPermissions,
        [property: JsonPropertyName("denyPermissions")] int DenyPermissions,
        [property: JsonPropertyName("identifier")] string Identifier,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("scope")] string Scope
    );

    public record AzDoTypeRoleAssignmentCollection(
        [property: JsonPropertyName("count")] int Count,
        [property: JsonPropertyName("value")] IReadOnlyList<AzDoTypeRoleAssignment> Value
    );

    public record AzDoTypeRoleAssignment(
        [property: JsonPropertyName("identity")] AzDoTypeIdentity Identity,
        [property: JsonPropertyName("role")] AzDoRoleDefinition Role,
        [property: JsonPropertyName("access")] string Access,
        [property: JsonPropertyName("accessDisplayName")] string AccessDisplayName
    );

}