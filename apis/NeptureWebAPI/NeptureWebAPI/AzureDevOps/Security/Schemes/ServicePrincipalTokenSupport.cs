

using Azure.Core;
using Azure.Identity;

namespace NeptureWebAPI.AzureDevOps.Security.Schemes
{
    public class ServicePrincipalTokenSupport
    {
        private readonly AppConfig appConfig;
        private TokenCredential? tokenCredential;

        public ServicePrincipalTokenSupport(AppConfig appConfig)
        {
            this.appConfig = appConfig;
            if(IsConfigured())
            {
                tokenCredential = new ClientSecretCredential(appConfig.TenantId, appConfig.ClientId, appConfig.ClientSecret);
            }
        }

        public async Task<(string, string)> GetCredentialsAsync()
        {
            if(tokenCredential == null)
            {
                throw new InvalidOperationException("Service Principal Authentication is not configured but attempting to use.");
            }

            // Whenever possible, credential instance should be reused for the lifetime of the process.
            // An internal token cache is used which reduces the number of outgoing calls to Azure AD to get tokens.
            // Call GetTokenAsync whenever you are making a request. Token caching and refresh logic is handled by the credential object.
            var tokenRequestContext = new TokenRequestContext(VssAadSettings.DefaultScopes);
            var accessToken = await tokenCredential.GetTokenAsync(tokenRequestContext, CancellationToken.None);
            return ("Bearer", accessToken.Token);
        }

        public bool IsConfigured()
        {
            var hasClientId = !string.IsNullOrWhiteSpace(appConfig.ClientId);
            var hasClientSecret = !string.IsNullOrWhiteSpace(appConfig.ClientSecret);
            var hasTenantId = !string.IsNullOrWhiteSpace(appConfig.TenantId);

            return hasClientId && hasClientSecret && hasTenantId;
        }
    }
}
