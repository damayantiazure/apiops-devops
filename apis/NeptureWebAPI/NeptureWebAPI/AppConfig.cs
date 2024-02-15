
using System.Drawing.Printing;

namespace NeptureWebAPI
{
    public class AppConfig
    {
        private const string APPINSIGHT_CONN_STR_KEY = "APPINSIGHT_CONN_STR";
        public const string AZUREDEVOPSCLIENT = "AZUREDEVOPSCLIENT";
        public const string AZUREDEVOPS_IDENTITY_CLIENT = "AZUREDEVOPS_IDENTITY_CLIENT";
        public const string AZDO_URI = "https://dev.azure.com";
        public const string AZDO_IDENTITY_URI = "https://vssps.dev.azure.com";
        private const string AZDO_ORG_KEY = "AZDO_ORG";
        private const string AZDO_PAT = "AZDO_PAT";
        private string orgName;
        private string appInsightConnStr;
        private string pat;

        private string clientId;
        private string clientSecret;
        private string tenantId;

        private bool useManagedIdentity = false;
        private string userAssignedIdentityId = "";

        public AppConfig()
        {
            var orgName = System.Environment.GetEnvironmentVariable(AZDO_ORG_KEY);
            ArgumentNullException.ThrowIfNullOrEmpty(orgName, $"Environment variable {AZDO_ORG_KEY} is not set");
            this.orgName = orgName;
            this.appInsightConnStr = (appInsightConnStr != null) ? appInsightConnStr : "NOT-SET";
            this.pat = System.Environment.GetEnvironmentVariable(AZDO_PAT);


            this.clientId = System.Environment.GetEnvironmentVariable("AZDO_CLIENT_ID");
            this.clientSecret = System.Environment.GetEnvironmentVariable("AZDO_CLIENT_SECRET");
            this.tenantId = System.Environment.GetEnvironmentVariable("AZDO_TENANT_ID");

            var anyValue = System.Environment.GetEnvironmentVariable("AZDO_USE_MANAGED_IDENTITY");
            useManagedIdentity = !string.IsNullOrWhiteSpace(anyValue);
            userAssignedIdentityId = System.Environment.GetEnvironmentVariable("AZDO_MANAGED_IDENTITY_ID");
        }

        public static string? GetAppInsightsConnStrFromEnv()
        {
            return System.Environment.GetEnvironmentVariable(APPINSIGHT_CONN_STR_KEY);
        }

        public string OrgName => orgName;
        public string AppInsightConnStr => appInsightConnStr;
        public string Pat => pat;

        public string ClientId => clientId;
        public string ClientSecret => clientSecret;
        public string TenantId => tenantId;

        public bool UseManagedIdentity => useManagedIdentity;
        public string UserAssignedIdentityId => userAssignedIdentityId;
    }
}