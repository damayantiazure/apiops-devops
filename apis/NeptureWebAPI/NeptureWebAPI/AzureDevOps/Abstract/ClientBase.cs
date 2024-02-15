

using NeptureWebAPI.AzureDevOps.Security;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NeptureWebAPI.AzureDevOps.Abstract
{
    public abstract class ClientBase
    {
        protected readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly IHttpContextAccessor httpContextAccessor;
        protected readonly AppConfig appConfiguration;
        private readonly IdentitySupport identitySupport;
        protected readonly IHttpClientFactory httpClientFactory;

        public ClientBase(
            JsonSerializerOptions jsonSerializerOptions,
            IHttpContextAccessor httpContextAccessor,
            AppConfig appConfiguration,
            IdentitySupport identitySupport,
            IHttpClientFactory httpClientFactory)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.httpContextAccessor = httpContextAccessor;
            this.appConfiguration = appConfiguration;
            this.identitySupport = identitySupport;
            this.httpClientFactory = httpClientFactory;
        }

        protected async virtual Task<TResponsePayload> PostAsync<TRequestPayload, TResponsePayload>(
            string apiPath, TRequestPayload payload, bool elevate = false) 
            where TRequestPayload : class 
            where TResponsePayload : class
        {
            return await SendRequestCoreAsync<TRequestPayload, TResponsePayload>(AppConfig.AZUREDEVOPSCLIENT, apiPath, payload, HttpMethod.Post, elevate);
        }

        protected async virtual Task<TResponsePayload> PutAsync<TRequestPayload, TResponsePayload>(
            string apiPath, TRequestPayload payload, bool elevate = false)
            where TRequestPayload : class
            where TResponsePayload : class
        {
            return await SendRequestCoreAsync<TRequestPayload, TResponsePayload>(AppConfig.AZUREDEVOPSCLIENT, apiPath, payload, HttpMethod.Put, elevate);
        }

        protected async virtual Task<TResponsePayload> PatchAsync<TRequestPayload, TResponsePayload>(
            string apiPath, TRequestPayload payload, bool elevate = false)
            where TRequestPayload : class
            where TResponsePayload : class
        {
            return await SendRequestCoreAsync<TRequestPayload, TResponsePayload>(AppConfig.AZUREDEVOPSCLIENT, apiPath, payload, HttpMethod.Patch, elevate);
        }

        protected async virtual Task<bool> PatchWithoutBodyAsync(
            string apiPath, bool elevate = false)
        {
            return await SendRequestWithoutBodyCoreAsync(AppConfig.AZUREDEVOPSCLIENT, apiPath, HttpMethod.Patch, elevate);
        }

        

        protected async virtual Task<TPayload> GetAsync<TPayload>(string apiPath, bool elevate = false) where TPayload : class
        {
            return await GetCoreAsync<TPayload>(AppConfig.AZUREDEVOPSCLIENT, apiPath, elevate);
        }

        protected async virtual Task<TPayload> GetVsspAsync<TPayload>(string apiPath, bool elevate = false) where TPayload : class
        {
            return await GetCoreAsync<TPayload>(AppConfig.AZUREDEVOPS_IDENTITY_CLIENT, apiPath, elevate);
        }

        protected async virtual Task<TResponsePayload> PostVsspAsync<TRequestPayload, TResponsePayload>(
            string apiPath, TRequestPayload payload, bool elevate = false)
            where TRequestPayload : class
            where TResponsePayload : class
        {
            return await SendRequestCoreAsync<TRequestPayload, TResponsePayload>(AppConfig.AZUREDEVOPS_IDENTITY_CLIENT, apiPath, payload, HttpMethod.Post, elevate);
        }

        private async Task<TPayload> GetCoreAsync<TPayload>(
            string apiType, string apiPath, bool elevate = false) where TPayload : class
        {
            var (scheme, token) = await identitySupport.GetCredentialsAsync(elevate);
            using HttpClient client = httpClientFactory.CreateClient(apiType);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            var path = $"/{appConfiguration.OrgName}/{apiPath}";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var x = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<TPayload>(this.jsonSerializerOptions);
                if (result != null)
                {
                    return result;
                }
            }
            throw new InvalidOperationException($"Error: {response.StatusCode}");
        }

        private async Task<TResponsePayload> SendRequestCoreAsync<TRequestPayload, TResponsePayload>(
            string apiType, string apiPath, TRequestPayload payload, HttpMethod httpMethod, bool elevate = false) 
            where TRequestPayload : class
            where TResponsePayload : class
        {
            var (scheme, token) = await identitySupport.GetCredentialsAsync(elevate);
            using HttpClient client = httpClientFactory.CreateClient(apiType);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            var path = $"/{appConfiguration.OrgName}/{apiPath}";
            
            using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync<TRequestPayload>(memoryStream, payload, this.jsonSerializerOptions);

            var jsonContent = new StringContent(Encoding.UTF8.GetString(memoryStream.ToArray()) , Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(httpMethod, path) 
            {
                Content = jsonContent
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var x = await response.Content.ReadAsStringAsync();
                if(typeof(TResponsePayload) == typeof(string))
                {   
                    return (!string.IsNullOrWhiteSpace(x as string) ? x as string : string.Empty) as TResponsePayload;
                }
                var result = await response.Content.ReadFromJsonAsync<TResponsePayload>(this.jsonSerializerOptions);
                if (result != null)
                {
                    return result;
                }
            }
            throw new InvalidOperationException($"Error: {response.StatusCode}");
        }

        private async Task<bool> SendRequestWithoutBodyCoreAsync(
            string apiType, string apiPath, HttpMethod httpMethod, bool elevate = false)
        {
            var (scheme, token) = await identitySupport.GetCredentialsAsync(elevate);
            using HttpClient client = httpClientFactory.CreateClient(apiType);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            var path = $"/{appConfiguration.OrgName}/{apiPath}";
            
            var request = new HttpRequestMessage(httpMethod, path);
            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


        protected string GetOrgName()
        {
            return appConfiguration.OrgName;
        }
    }
}
