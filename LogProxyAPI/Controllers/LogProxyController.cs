using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogProxyAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LogProxyController : ControllerBase
    {
     
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public LogProxyController(IConfiguration configuration)
        {
            var thirdPartyAPI = configuration.GetSection("thirdPartyPIDetails");
            _baseUrl = thirdPartyAPI["baseUrl"];
            _apiKey = thirdPartyAPI["apiKey"];
        }

        
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string urlGetLog = $"{_baseUrl}?maxRecords=3&view=Grid%20view";
                    client.BaseAddress = new Uri(urlGetLog);

                    //Add request header
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

                    //Send request
                    var response = await client.GetAsync(urlGetLog);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<string> Post(Object proxyLog)
        {
            using (HttpClient client = new HttpClient())
            {
                
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

                string requestData = JsonSerializer.Serialize(proxyLog);
                var buffer = System.Text.Encoding.UTF8.GetBytes(requestData);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                
                //Send request
                var response = await client.PostAsync(_baseUrl, byteContent);

                var jresponse = Ok(new { jobResult = response });
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
