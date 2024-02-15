

using System.Text.Json.Serialization;

namespace NeptureWebAPI.AzureDevOps.Payloads
{
    public record AzDoClassificationNodePayload(
        [property: JsonPropertyName("operationData")] string OperationData,
        [property: JsonPropertyName("syncWorkItemTracking")] bool SyncWorkItemTracking
    );

    public record AzDoClassificationNodeDetailsInPayload(
        [property: JsonPropertyName("NodeName")] string NodeName,
        [property: JsonPropertyName("ParentId")] string ParentId
    );


    public record AzDoClassificationNode(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("parentId")] string ParentId,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("children")] List<AzDoClassificationNode> Children,
        [property: JsonPropertyName("values")] List<string> Values
    );

    public record AzDoClassificationNodeCreatedResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("node")] AzDoClassificationNode Node
    );








    public record ClassificationNode(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("identifier")] string Identifier,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("structureType")] string StructureType,
        [property: JsonPropertyName("hasChildren")] bool HasChildren,
        [property: JsonPropertyName("children")] IReadOnlyList<ClassificationNode> Children,
        [property: JsonPropertyName("path")] string Path,        
        [property: JsonPropertyName("url")] string Url
    );
}