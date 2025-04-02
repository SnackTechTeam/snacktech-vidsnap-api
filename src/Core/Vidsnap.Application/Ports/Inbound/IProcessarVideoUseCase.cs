using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;

namespace Vidsnap.Application.Ports.Inbound
{
    public interface IProcessarVideoUseCase
    {        
        Task<ResultadoOperacao<NovoVideoResponse>> EnviarVideoParaProcessamentoAsync(NovoVideoRequest novoVideoRequest);        
    }
}