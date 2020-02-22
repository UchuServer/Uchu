using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Uchu.Sso
{
    public class SsoService
    {
        public HttpClient Client { get; }
        
        public string SsoDomain { get; }

        public SsoService(string domain)
        {
            Client = new HttpClient();

            SsoDomain = domain;
        }

        public async Task<bool> VerifyAsync(string username, string key)
        {
            if (string.IsNullOrWhiteSpace(SsoDomain)) return false;

            try
            {
                var request = $"https://{SsoDomain}:21835/verify/{HttpUtility.UrlEncode(username)}/{HttpUtility.UrlEncode(key)}";

                var result = await Client.GetStringAsync(request);

                return result == "1";
            }
            catch
            {
                return false;
            }
        }
    }
}