using FluentAssertions;
using System.Net.Http.Json;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.BddTest.Support;

namespace Vidsnap.BddTest.StepDefinitions
{
    [Binding]
    public sealed class VideoRegistrationStepDefinitions(CustomWebApplicationFactory factory, ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private readonly HttpClient _client = factory.CreateClient();

        [Given(@"I have the following invalid video data:")]
        public void GivenIHaveTheFollowingVideoData(Table table)
        {
            var row = table.Rows[0];
            var request = new NovoVideoRequest(
                Guid.Parse(row["IdUsuario"]),
                row["EmailUsuario"],
                row["NomeVideo"],
                row["Extensao"],
                int.Parse(row["Tamanho"]),
                int.Parse(row["Duracao"])
            );

            _scenarioContext["Request"] = request;
        }

        [Given(@"I have the following valid video data:")]
        public void GivenIHaveTheFollowingValidVideoData(Table table)
        {
            var row = table.Rows.First();
            var request = new NovoVideoRequest(
                Guid.Parse(row["IdUsuario"]),
                row["EmailUsuario"],
                row["NomeVideo"],
                row["Extensao"],
                int.Parse(row["Tamanho"]),
                int.Parse(row["Duracao"])
            );

            _scenarioContext["Request"] = request;
        }

        [When("I POST this data to {string}")]
        public async Task WhenIPOSTThisDataTo(string endpoint)
        {
            var request = _scenarioContext.Get<NovoVideoRequest>("Request");
            var response = await _client.PostAsJsonAsync(endpoint, request);
            _scenarioContext["Response"] = response;
        }

        [Then(@"the response status code should be (.*)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            ((int)response.StatusCode).Should().Be(expectedStatusCode);
        }

        [Then("the response should contain the error {string}")]
        public async Task ThenTheResponseShouldContainTheError(string expectedMessage)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(expectedMessage);
        }

        [Then("the response should contain a valid video registration confirmation")]
        public async Task ThenTheResponseShouldContainAValidVideoRegistrationConfirmation()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<NovoVideoResponse>();
            responseData.Should().NotBeNull();
            responseData.Id.Should().NotBeEmpty();
            responseData.StatusAtual.Should().Be("Recebido");
            responseData.UrlPreAssinadaDeUpload.Should().NotBeEmpty();
        }
    }
}
