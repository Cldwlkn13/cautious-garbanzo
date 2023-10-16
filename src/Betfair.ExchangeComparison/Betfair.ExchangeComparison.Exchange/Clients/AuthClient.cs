using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization.Json;
using System.Text;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Exchange.Clients
{
    public class AuthClient : IAuthClient
    {
        private string appKey;

        public string AppKey
        {
            get { return appKey; }
        }

        public AuthClient()
        {
            this.appKey = "OOPwCm6u41IrQ8cA";
        }

        private HttpClientHandler getWebRequestHandlerWithCert(string certFilename)
        {
            //var cert = new X509Certificate2(certFilename, "");
            var clientHandler = new HttpClientHandler();
            //clientHandler.ClientCertificates.Add(cert);
            return clientHandler;
        }

        private const string DEFAULT_COM_BASEURL = "https://identitysso.betfair.com/";

        private HttpClient initHttpClientInstance(HttpClientHandler clientHandler, string appKey, string baseUrl = DEFAULT_COM_BASEURL)
        {
            var client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-Application", appKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private FormUrlEncodedContent getLoginBodyAsContent(string username, string password)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("username", username));
            postData.Add(new KeyValuePair<string, string>("password", password));
            return new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
        }

        public KeepAliveLogoutResponse Login(string username, string password)
        {
            username = "TradingProdTest17";
            password = "Trading_Prod_17";

            var handler = getWebRequestHandlerWithCert("");
            var client = initHttpClientInstance(handler, appKey);
            var content = getLoginBodyAsContent(username, password);
            var result = client.PostAsync($"api/login?username={username}&password={password}", content).Result;
            result.EnsureSuccessStatusCode();
            var jsonSerialiser = new DataContractJsonSerializer(typeof(KeepAliveLogoutResponse));
            var stream = new MemoryStream(result.Content.ReadAsByteArrayAsync().Result);
            return (KeepAliveLogoutResponse)jsonSerialiser.ReadObject(stream);
        }
    }
}
