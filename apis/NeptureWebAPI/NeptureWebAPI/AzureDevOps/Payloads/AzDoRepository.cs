using System.Text.Json.Serialization;

namespace NeptureWebAPI.AzureDevOps.Payloads
{
    public record AzDoProject(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("revision")] int Revision,
        [property: JsonPropertyName("visibility")] string Visibility,
        [property: JsonPropertyName("lastUpdateTime")] DateTime LastUpdateTime
    );

    public record AzDoRepository(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("project")] AzDoProject Project,
        [property: JsonPropertyName("size")] int Size,
        [property: JsonPropertyName("remoteUrl")] string RemoteUrl,
        [property: JsonPropertyName("sshUrl")] string SshUrl,
        [property: JsonPropertyName("webUrl")] string WebUrl,
        [property: JsonPropertyName("isDisabled")] bool IsDisabled,
        [property: JsonPropertyName("isInMaintenance")] bool IsInMaintenance
    );
}
