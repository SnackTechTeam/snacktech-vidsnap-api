using System.Text;
using System.Text.Json;
using Vidsnap.BddTest.Support;

namespace Vidsnap.BddTest.Drivers
{
    [Binding]
    public class ApiDriver
    {
        private readonly ScenarioContext _context;
        private readonly CustomWebApplicationFactory _factory;
        public HttpClient Client { get; }

        public ApiDriver(ScenarioContext context)
        {
            _context = context;
            _factory = new CustomWebApplicationFactory();
            Client = _factory.CreateClient();
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content);
            _context["LastResponse"] = response;
            return response;
        }

        public HttpResponseMessage? GetLastResponse() => _context["LastResponse"] as HttpResponseMessage;
    }
}
