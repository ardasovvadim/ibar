using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.Services.FileServices.Mock
{
    public class ZohoApiMockService : IRestApiService
    {
        public bool FileUpload(string fileName, Stream fileStream)
        {
            Thread.Sleep(1000);
            return true;
        }

        public async Task<bool> FileUploadAsync(string fileName, Stream fileStream)
        {
            Thread.Sleep(1000);
            return await Task.FromResult(true);
        }

        public async Task<HttpResponseMessage> GetResponseOfUploadAsync(string fileName, Stream fileStream)
        {
            var response = new HttpResponseMessage()
            {
                ReasonPhrase = "MOCK phrase",
                StatusCode = HttpStatusCode.OK,
                RequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://mock.mock")
            };
            return await Task.FromResult(response);
        }
    }
}
