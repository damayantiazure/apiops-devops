

using Microsoft.ApplicationInsights.AspNetCore;
using NeptureWebAPI.AzureDevOps.Abstract;
using NeptureWebAPI.AzureDevOps.Payloads;
using NeptureWebAPI.AzureDevOps.Security;
using NeptureWebAPI.Controllers;
using System.Text.Json;

namespace NeptureWebAPI.AzureDevOps
{
    public class Client : ClientBase
    {
        private readonly ILogger<Client> logger;

        public Client(
            IHttpContextAccessor httpContextAccessor,
            JsonSerializerOptions jsonSerializerOptions,
            AppConfig appConfiguration,
            IHttpClientFactory httpClientFactory,
            IdentitySupport identitySupport,
            ILogger<Client> logger) : base(jsonSerializerOptions, httpContextAccessor, 
                appConfiguration, identitySupport, httpClientFactory)
        {
            this.logger = logger;
        }

        public async Task<string> GetHealthInfo()
        {
            var orgName = this.GetOrgName();
            return $"{orgName}";
        }

        public async Task<AzDoTeamCollection> GetTeamsAsync(bool mine = true, int top = 10, int skip = 0)
        {
            return await this.GetAsync<AzDoTeamCollection>($"_apis/teams?$mine={mine}&$top={top}&$skip={skip}&api-version=7.0-preview.3");
        }

        public async Task<AzDoConnectionData> GetConnectionDataAsync(bool elevated = false)
        {
            return await this.GetAsync<AzDoConnectionData>($"_apis/connectionData", elevated);
        }
        

        public async Task<AzDoConnectionData> GetProjectsAsync(bool elevated = false)
        {
            return await this.GetAsync<AzDoConnectionData>($"_apis/projects?api-version=7.2-preview.4", elevated);
        }

        public async Task<AzDoGroupMembershipSlimCollection> GetGroupMembershipsAsync(string subjectDescriptor)
        {
            return await this.GetVsspAsync<AzDoGroupMembershipSlimCollection>($"_apis/graph/Memberships/{subjectDescriptor}?api-version=7.0-preview.1");
        }

        public async Task<ClassificationNode> GetClassificationNodeAsync(string projectId, bool isAreaPath)
        {            
            var depth = 10;
            var path = $"{projectId}/_apis/wit/classificationnodes/{(isAreaPath ? "Areas": "iterations")}?$depth={depth}&api-version=7.0";            
            var node = await GetAsync<ClassificationNode>(path, true);
            return node;
        }

        public async Task<AzDoClassificationNodeCreatedResponse> CreateNewClassificationNodeAsync(NewNodePayload newNodePayload)
        {
            var path = $"{newNodePayload.projectId}/_admin/_Areas/CreateClassificationNode?useApiUrl=true&__v=5";
            var opBody = new AzDoClassificationNodeDetailsInPayload(NodeName: newNodePayload.nodeName, ParentId: newNodePayload.parentId);
            var operation = JsonSerializer.Serialize(opBody, jsonSerializerOptions);

            var payload = new AzDoClassificationNodePayload(OperationData: operation, SyncWorkItemTracking: false);
            return await PostAsync<AzDoClassificationNodePayload, AzDoClassificationNodeCreatedResponse>(path, payload, true);
        }

        public async Task<bool> ApplyAcksAsync(string namespaceId, AzDoAclEntryCollection[] aces)
        {
            var path = $"_apis/accesscontrollists/{namespaceId}?api-version=6.0";
            await PostAsync<AzDoAclEntryPostBody, string>(path, new AzDoAclEntryPostBody(aces), true);
            return true;
        }

        public async Task<bool> UpdateRoleAssginmentAsync(string apiVersion, string projectId, string resourceId, string seperator, string scope, AzDoRoleAssignment[] body)
        {
            var path = $"_apis/securityroles/scopes/{scope}/roleassignments/resources/{projectId}{seperator}{resourceId}?api-version={apiVersion}";
            await PutAsync<AzDoRoleAssignment[], string>(path, body, true);
            return true;
        }

        public async Task<bool> UpdateRoleInheritanceAsync(string apiVersion, string projectId, string resourceId, string seperator, string scope, bool inheritPermissions)
        {
            var path = $"_apis/securityroles/scopes/{scope}/roleassignments/resources/{projectId}{seperator}{resourceId}?api-version={apiVersion}&inheritPermissions={inheritPermissions}";
            return await PatchWithoutBodyAsync(path, true);
        }

        public async Task<AzDoTypeRoleAssignment[]> GetRoleAssignmentAsync(string apiVersion, string projectId, string resourceId, string seperator, string scope)
        {
            var path = $"_apis/securityroles/scopes/{scope}/roleassignments/resources/{projectId}{seperator}{resourceId}?api-version={apiVersion}";
            var roleAssignments = await GetAsync<AzDoTypeRoleAssignmentCollection>(path, true);
            if(roleAssignments != null && roleAssignments.Value != null)
            {
                return roleAssignments.Value.ToArray();
            }
            return new List<AzDoTypeRoleAssignment>().ToArray();
        }

        public async Task<bool> DeleteRoleAssignmentAsync(string apiVersion, string projectId, string resourceId, string seperator, string scope, string[] identities)
        {
            var path = $"_apis/securityroles/scopes/{scope}/roleassignments/resources/{projectId}{seperator}{resourceId}?api-version={apiVersion}";
            await PatchAsync<string[], string>(path, identities, true);

            return true;
        }

        public async Task<AzDoRepository> CreateRepositoryAsync(string projectId, string repositoryName)
        {
            var path = $"{projectId}/_apis/git/Repositories?api-version=5.0-preview.1";
            var payload = new 
            {
                name = repositoryName,
                project = new { id = projectId }
            };
            var repo = await PostAsync<object, AzDoRepository>(path, payload, true);
            return repo;
        }

        public async Task<IEnumerable<AzDoIdentity>> SearchIdentityAsync(IdentitySearchPayload payload)
        {
            var identities = new List<AzDoIdentity>();
            var path = $"_apis/IdentityPicker/Identities?api-version=5.0-preview.1";
            var saerchResult = await PostAsync<IdentitySearchPayload, AzDoSearchResponse>(path, payload, true);
            if(saerchResult != null && saerchResult.Results != null )
            {
                foreach(var result in saerchResult.Results)
                {
                    if(result.Identities != null)
                    {
                        identities.AddRange(result.Identities);
                    }
                }
            }
            return identities;
        }

        public async Task<AzDoIdentity> MaterializeGroupAsync(AzDoIdentity group)
        {
            var path = $"_apis/graph/groups?api-version=6.0-preview.1";
            var payload = new
            {
                originId = group.OriginId
            };
            var materializedResponse = await PostVsspAsync<object, AzDoGroupMaterializeResponse>(path, payload, true);
            
            var newGroup = new AzDoIdentity(group.EntityId, group.EntityType, group.OriginDirectory, group.OriginId, materializedResponse.Domain,
                group.LocalId, materializedResponse.PrincipalName, group.ScopeName, group.SamAccountName, materializedResponse.Descriptor, group.Department,
                group.JobTitle, group.Mail, group.MailNickname, group.PhysicalDeliveryOfficeName, group.SignInAddress, group.Surname, group.Guest,
                group.Description, group.IsMru);
            return newGroup;
        }

        public async Task<IReadOnlyList<AzDoTranslatedIdentityDescriptor>> TranslateDescriptorsAsync(string subjectDescriptors)
        {
            var path = $"_apis/identities?api-version=7.0&subjectDescriptors={subjectDescriptors}&queryMembership=None";

            var response = await GetVsspAsync<AzDoDescriptorTranslationResponse>(path, true);
            return response.Value;            
        }
    }
}
