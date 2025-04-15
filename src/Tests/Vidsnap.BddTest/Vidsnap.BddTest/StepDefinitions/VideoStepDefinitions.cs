using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net.Http.Json;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.BddTest.Helpers;
using Vidsnap.BddTest.Support;

namespace Vidsnap.BddTest.StepDefinitions
{
    [Binding]
    public sealed class VideoStepDefinitions(CustomWebApplicationFactory factory, ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private readonly CustomWebApplicationFactory _factory = factory;
        private readonly HttpClient _client = factory.CreateClient();

        #region Steps Video Registration

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

        #endregion

        #region Steps Video Listing

        [Given("the following videos have been registered:")]
        public async Task GivenTheFollowingVideosHaveBeenRegistered(Table table)
        {
            foreach (var row in table.Rows)
            {
                var request = new NovoVideoBodyRequest(
                    row["NomeVideo"],
                    row["Extensao"],
                    int.Parse(row["Tamanho"]),
                    int.Parse(row["Duracao"])
                );

                _client.DefaultRequestHeaders.Add("X-User-Id", row["IdUsuario"]);
                _client.DefaultRequestHeaders.Add("X-User-Email", row["EmailUsuario"]);

                var response = await _client.PostAsJsonAsync("/api/videos", request);
                response.EnsureSuccessStatusCode();

                //Remove os header para o novo loop
                _client.DefaultRequestHeaders.Remove("X-User-Id");
                _client.DefaultRequestHeaders.Remove("X-User-Email");
            }
        }

        [When(@"I GET videos for user with ID ""(.*)""")]
        public async Task WhenIGetVideosForUserWithId(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/videos");
            request.Headers.Add("X-User-Id", userId);

            var response = await _client.SendAsync(request);
            _scenarioContext["Response"] = response;
        }

        [Then(@"the response should contain (\d+) videos")]
        public async Task ThenTheResponseShouldContainVideos(int expectedCount)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var content = await response.Content.ReadFromJsonAsync<List<VideoResponse>>();
            content.Should().NotBeNull();
            content!.Count.Should().Be(expectedCount);

            _scenarioContext["Videos"] = content;
        }

        [Then(@"one of the videos should be named ""(.*)""")]
        public void ThenOneOfTheVideosShouldBeNamed(string expectedName)
        {
            var videos = _scenarioContext.Get<List<VideoResponse>>("Videos");
            videos.Should().Contain(v => v.Nome == expectedName);
        }

        #endregion

        #region Video Status Update

        [When("I receive a message with status {string}")]
        public void WhenIReceiveAMessageWithStatusProcessando(string status)
        {
            var novoVideoResponse = _scenarioContext.Get<NovoVideoResponse>("NovoVideoResponse");

            var mockMessage = new AtualizaStatusVideoRequest(novoVideoResponse.Id, status);
            _scenarioContext["ProcessandoStatus"] = mockMessage;
        }

        [When("I successfuly process the message")]
        public async Task WhenISuccessfulyProcessTheMessage()
        {
            var mockMessage = _scenarioContext.Get<AtualizaStatusVideoRequest>("ProcessandoStatus");

            var newFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.ReplaceMessageQueueServiceWithMock(mockMessage);
                });
            });

            await WorkerTestHelper.ExecutarWorkerAtualizacaoStatusAsync(newFactory.Services);
        }

        [Then("the current status should be {string}")]
        public void ThenTheCurrentStatusShouldBe(string status)
        {
            var video = _scenarioContext.Get<VideoResponse>("VideoResponse");

            video.StatusAtual.Should().Be(status);
            video.StatusHistory.FirstOrDefault(v => v.Status ==  status).Should().NotBeNull();
        }

        #endregion

        #region Steps Links Download

        [Given("I update the video status to {string}")]
        public async Task GivenIUpdateTheVideoStatusTo(string status)
        {
            var novoVideoResponse = _scenarioContext.Get<NovoVideoResponse>("NovoVideoResponse");

            var mockMessage = new AtualizaStatusVideoRequest(
                novoVideoResponse.Id, 
                status,
                "s3://bucket/zip.zip",
                "s3://bucket/image.jpg");

            var newFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.ReplaceMessageQueueServiceWithMock(mockMessage);
                });
            });

            await WorkerTestHelper.ExecutarWorkerAtualizacaoStatusAsync(newFactory.Services);
        }

        [When(@"I GET the download links for this video")]
        public async Task WhenIGetTheDownloadLinksForThisVideo()
        {
            var video = _scenarioContext.Get<NovoVideoResponse>("NovoVideoResponse");

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/videos/{video.Id}");
            request.Headers.Add("X-User-Id", video.IdUsuario.ToString());

            var response = await _client.SendAsync(request);
            _scenarioContext["Response"] = response;
        }

        [Then(@"the response should contain a download link with zip and image URLs")]
        public async Task ThenTheResponseShouldContainDownloadLinks()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var content = await response.Content.ReadFromJsonAsync<LinksDeDownloadResponse>();

            content.Should().NotBeNull();
            content.URLZip.Should().NotBeNullOrWhiteSpace("a zip download URL should be present");
            content.URLImagem.Should().NotBeNullOrWhiteSpace("an image preview URL should be present");
            content.DataExpiracao.Should().BeAfter(DateTime.Now.AddMinutes(-1), "expiration must be in the future");
        }

        #endregion

        #region Common Steps

        [Given(@"I register this video")]
        public async Task GivenIRegisterThisVideo()
        {
            var request = _scenarioContext.Get<NovoVideoBodyRequest>("Request");
            var idUsuarioRequest = _scenarioContext.Get<Guid>("IdUsuarioRequest");
            var emailUsuarioRequest = _scenarioContext.Get<string>("EmailUsuarioRequest");

            _client.DefaultRequestHeaders.Add("X-User-Id", idUsuarioRequest.ToString());
            _client.DefaultRequestHeaders.Add("X-User-Email", emailUsuarioRequest);

            var response = await _client.PostAsJsonAsync("/api/videos", request);

            _scenarioContext["NovoVideoResponse"] = await response.Content.ReadFromJsonAsync<NovoVideoResponse>();
            response.EnsureSuccessStatusCode();
        }

        [Then(@"the response status code should be (.*)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            ((int)response.StatusCode).Should().Be(expectedStatusCode);
        }

        [When("I GET the video by name {string}")]
        public async Task WhenIGETTheVideoByName(string name)
        {
            var idUsuarioRequest = _scenarioContext.Get<Guid>("IdUsuarioRequest");

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/videos");
            request.Headers.Add("X-User-Id", idUsuarioRequest.ToString());

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<List<VideoResponse>>();
            _scenarioContext["VideoResponse"] = content!.FirstOrDefault(v => v.Nome == name);
        }

        #endregion
    }
}
