using IBAR.TradeModel.Business.Common;
using IBAR.TradeModel.Business.Common.Extensions;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.Services.FileServices
{
    public interface IRestApiService
    {
        Task<HttpResponseMessage> GetResponseOfUploadAsync(string fileName, Stream fileStream);
        Task<bool> FileUploadAsync(string fileName, Stream fileStream);
        bool FileUpload(string fileName, Stream fileStream);
    }

    public class ZohoApiService : IRestApiService
    {
        private readonly HttpClient _httpClient;

        private readonly string _apiTokenUrl;
        private readonly string _apiUploadUrl;

        private string _accessToken;
        public string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_accessToken))
                {
                    using (var client = new HttpClient())
                    {
                        var response = client.GetAsync(_apiTokenUrl).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            dynamic token = response.Content.ReadAsAsync<ExpandoObject>().Result;
                            _accessToken = token.details.output;
                        }
                    }
                }

                return _accessToken;
            }
        }

        public ZohoApiService()
        {
            _apiTokenUrl = ConfigurationManager.AppSettings["zoho:ApiTokenUrl"];
            _apiUploadUrl = ConfigurationManager.AppSettings["zoho:ApiUploadUrl"];

            if (string.IsNullOrEmpty(_apiTokenUrl))
                throw new ConfigurationErrorsException("Please add 'zohoApiTokenUrl' settigns to .config file.");

            if (string.IsNullOrEmpty(_apiUploadUrl))
                throw new ConfigurationErrorsException("Please add 'zohoApiUploadUrl' settigns to .config file.");

            _httpClient = HttpClientFactory.Create();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Zoho-oauthtoken {AccessToken}");
        }

        public async Task<HttpResponseMessage> GetResponseOfUploadAsync(string fileName, Stream fileStream)
        {
            fileName = TradeFileHelper.GetCorrectFileName(fileName);
            var fileContent = new ByteArrayContent(fileStream.GetByteArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = fileName, Name = "content" };
            return await _httpClient.PostAsync($"{_apiUploadUrl}{fileName}", new MultipartFormDataContent() { fileContent });
        }

        public async Task<bool> FileUploadAsync(string fileName, Stream fileStream)
        {
            fileName = TradeFileHelper.GetCorrectFileName(fileName);
            var fileContent = new ByteArrayContent(fileStream.GetByteArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = fileName, Name = "content" };
            var response = await _httpClient.PostAsync($"{_apiUploadUrl}{fileName}", new MultipartFormDataContent() { fileContent });

            return response.IsSuccessStatusCode;
        }

        public bool FileUpload(string fileName, Stream fileStream)
        {
            fileName = TradeFileHelper.GetCorrectFileName(fileName);
            var fileContent = new ByteArrayContent(fileStream.GetByteArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = fileName, Name = "content" };
            var response = _httpClient.PostAsync($"{_apiUploadUrl}{fileName}", new MultipartFormDataContent() { fileContent }).Result;

            return response.IsSuccessStatusCode;
        }
    }
}