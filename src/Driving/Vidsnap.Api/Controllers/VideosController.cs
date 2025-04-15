using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.Ports.Inbound;

namespace Vidsnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController(
        ILogger<IProcessarVideoUseCase> _logger, 
        IProcessarVideoUseCase _processarVideoUseCase,
        IBuscarVideosUseCase _buscarVideosUseCase) : CustomBaseController(_logger)
    {
        /// <summary>
        /// Cadastra um novo vídeo para processamento.
        /// </summary>
        /// <remarks>
        /// Cadastra um novo vídeo para processamento.
        /// </remarks>
        /// <param name="idUsuario">Identificador do usuário</param>
        /// <param name="emailUsuario">Email do usuário</param>
        /// <param name="requestBody">Os dados do video a ser processado.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação.</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<NovoVideoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Cadastra um novo vídeo para processamento.")]
        public async Task<IActionResult> Post(
            [FromHeader(Name = "X-User-Id")][Required] Guid idUsuario,
            [FromHeader(Name = "X-User-Email")][Required] string emailUsuario,
            [FromBody] NovoVideoBodyRequest requestBody)
        {
             var useCaseRequest = new NovoVideoRequest(
                 idUsuario,
                 emailUsuario ?? "",
                 requestBody.NomeVideo,
                 requestBody.Extensao,
                 requestBody.Tamanho,
                 requestBody.Duracao
             );
             return await ExecucaoPadrao(
                "Videos.Post", 
                _processarVideoUseCase.EnviarVideoParaProcessamentoAsync(useCaseRequest)
            );
        } 

        /// <summary>
        /// Busca todos os vídeos cadastrados pelo usuário.
        /// </summary>
        /// <remarks>
        /// Busca todos os vídeos cadastrados pelo usuário.
        /// </remarks>
        /// <param name="idUsuario">Identificador do usuário.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação.</returns>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<IReadOnlyList<VideoResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Lista todos os vídeo cadastrados pelo usuário")]
        public async Task<IActionResult> ObterVideosPorUsuario([FromHeader(Name = "X-User-Id")][Required] Guid idUsuario)
            => await ExecucaoPadrao(
                "Videos.ObterVideosPorUsuario", 
                _buscarVideosUseCase.ObterVideosDoUsuarioAsync(idUsuario)
            );

        /// <summary>
        /// Gera URLs de download dos arquivos processado de um vídeo. As URLs são geradas apenas para vídeos que finalizaram o processamento com sucesso
        /// </summary>
        /// <remarks>
        /// Gera URLs de download dos arquivos processado de um vídeo.
        /// </remarks>
        /// <param name="idUsuario">Identificador do usuário.</param>
        /// <param name="idVideo">Identificador do video.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação.</returns>
        [HttpGet("{id-video}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<IReadOnlyList<LinksDeDownloadResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Lista todos os vídeo cadastrados pelo usuário")]
        public async Task<IActionResult> ObterLinksDeDownloadDoVideo([FromHeader(Name = "X-User-Id")][Required]  Guid idUsuario, [FromRoute(Name = "id-video")] Guid idVideo)
            => await ExecucaoPadrao(
                "Videos.ObterLinksDeDownloadDoVideo",
                _buscarVideosUseCase.ObterLinksDeDownloadAsync(idVideo, idUsuario)
            );
    }
}