using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net.Http.Json;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.BddTest.Support;

namespace Vidsnap.BddTest.StepDefinitions
{
    [Binding]
    public sealed class VideoRegistrationStepDefinitions(CustomWebApplicationFactory factory, ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private readonly CustomWebApplicationFactory _factory = factory;
        private readonly HttpClient _client = factory.CreateClient();

        [Given(@"I have the following invalid video data:")]
        public void GivenIHaveTheFollowingVideoData(Table table)
        {
            var row = table.Rows[0];
            var request = new NovoVideoBodyRequest(
                row["NomeVideo"],
                row["Extensao"],
                int.Parse(row["Tamanho"]),
                int.Parse(row["Duracao"])
            );

            _scenarioContext["Request"] = request;
            _scenarioContext["IdUsuarioRequest"] = Guid.Parse(row["IdUsuario"]);
            _scenarioContext["EmailUsuarioRequest"] = row["EmailUsuario"];
        }

        [Given(@"I want to simulate a Internal Server Error")]
        public void GivenTheAPIWillThrowAnUnexpectedError()
        {
            var factoryWithError = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var mock = new Mock<IProcessarVideoUseCase>();
                    mock.Setup(x => x.EnviarVideoParaProcessamentoAsync(It.IsAny<NovoVideoRequest>()))
                        .ThrowsAsync(new Exception("Simulated error"));

                    services.RemoveAll<IProcessarVideoUseCase>();
                    services.AddTransient(_ => mock.Object);
                });
            });

            var client = factoryWithError.CreateClient();
            _scenarioContext["Client"] = client;
        }

        [Given(@"I have the following valid video data:")]
        public void GivenIHaveTheFollowingValidVideoData(Table table)
        {
            var row = table.Rows.First();
            var request = new NovoVideoBodyRequest(
                row["NomeVideo"],
                row["Extensao"],
                int.Parse(row["Tamanho"]),
                int.Parse(row["Duracao"])
            );

            _scenarioContext["Request"] = request;
            _scenarioContext["IdUsuarioRequest"] = Guid.Parse(row["IdUsuario"]);
            _scenarioContext["EmailUsuarioRequest"] = row["EmailUsuario"];
        }

        [When("I POST this data to {string}")]
        public async Task WhenIPOSTThisDataTo(string endpoint)
        {
            var request = _scenarioContext.Get<NovoVideoBodyRequest>("Request");
            var idUsuarioRequest = _scenarioContext.Get<Guid>("IdUsuarioRequest");
            var emailUsuarioRequest = _scenarioContext.Get<string>("EmailUsuarioRequest");

            var client = _scenarioContext.ContainsKey("Client")
                ? _scenarioContext.Get<HttpClient>("Client")
                : _client; // fallback para o padrão

            client.DefaultRequestHeaders.Add("X-User-Id", idUsuarioRequest.ToString());
            client.DefaultRequestHeaders.Add("X-User-Email", emailUsuarioRequest);

            var response = await client.PostAsJsonAsync(endpoint, request);
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
