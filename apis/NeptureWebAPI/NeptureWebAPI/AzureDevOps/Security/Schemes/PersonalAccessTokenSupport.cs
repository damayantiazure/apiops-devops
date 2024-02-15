using System.Text;

namespace NeptureWebAPI.AzureDevOps.Security.Schemes
{
    public class PersonalAccessTokenSupport
    {
        private readonly AppConfig config;

        public PersonalAccessTokenSupport(AppConfig config)
        {
            this.config = config;
        }

        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(config.Pat);
        }

        public (string, string) GetCredentials()
        {
            var base64Credential = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", config.Pat)));
            var scheme = "Basic";
            return (scheme, base64Credential);
        }
    }
}
