using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MetaWeblog.Portable.XmlRpc
{
    public class Service
    {
        private HttpClient _client;

        /// <summary>
        /// 
        /// </summary>
        public String Url { get; private set; }



        public Service(string url)
        {
            this.Url = url;

            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.ExpectContinue = false;
            _client.DefaultRequestHeaders.Add("user-agent", "MetaWeblogPortable");

        }

        public async Task<MethodResponse> Execute(MethodCall methodcall)
        {
            var doc = methodcall.CreateDocument();
            var stream = new MemoryStream();
            doc.Save(stream);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/xml") {CharSet = "utf-8"};

            var response = _client.PostAsync(Url, content);
            response.Result.EnsureSuccessStatusCode();

            var result = await response.Result.Content.ReadAsStringAsync();
            return new MethodResponse(result);
        }
    }
}