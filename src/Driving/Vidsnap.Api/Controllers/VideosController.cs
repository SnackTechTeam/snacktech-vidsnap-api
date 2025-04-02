using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        /// <param name="novoVideoRequest">Os dados do video a ser processado.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação.</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<NovoVideoResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Cadastra um novo vídeo para processamento.")]
        public async Task<IActionResult> Post([FromBody] NovoVideoRequest novoVideoRequest)
            => await ExecucaoPadrao(
                "Videos.Post", 
                _processarVideoUseCase.EnviarVideoParaProcessamentoAsync(novoVideoRequest)
            );

        /// <summary>
        /// Busca todos os vídeos cadastrados pelo usuário.
        /// </summary>
        /// <remarks>
        /// Busca todos os vídeos cadastrados pelo usuário.
        /// </remarks>
        /// <param name="idUsuario">Identificador do usuário.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação.</returns>
        [HttpGet("/usuario/{idUsuario}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<IReadOnlyList<VideoResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Lista todos os vídeo cadastrados pelo usuário")]
        public async Task<IActionResult> ObterVideosPorUsuario([FromRoute] Guid idUsuario)
            => await ExecucaoPadrao(
                "Videos.ObterVideosPorUsuario", 
                _buscarVideosUseCase.ObterVideosDoUsuarioAsync(idUsuario)
            );
    }
}