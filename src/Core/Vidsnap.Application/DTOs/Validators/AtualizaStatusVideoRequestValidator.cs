using FluentValidation;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Domain.Enums;

namespace Vidsnap.Application.DTOs.Validators
{
    public class AtualizaStatusVideoRequestValidator : AbstractValidator<AtualizaStatusVideoRequest>
    {
        public AtualizaStatusVideoRequestValidator()
        {
            RuleFor(x => x.IdVideo)
                .NotEmpty().WithMessage("O Id do vídeo é obrigatório.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("O NovoStatus é obrigatório.")
                .Must(status => Enum.TryParse(typeof(Status), status, out _))
                .WithMessage("O NovoStatus informado é inválido.");

            RuleFor(x => x.UrlZip)
                .Must((request, url) => string.IsNullOrWhiteSpace(url) || request.Status == Status.FinalizadoComSucesso.ToString())
                .WithMessage("A UrlZip só pode ser preenchida quando o status for 'FinalizadoComSucesso'.");

            RuleFor(x => x.UrlImagem)
                .Must((request, url) => string.IsNullOrWhiteSpace(url) || request.Status == Status.FinalizadoComSucesso.ToString())
                .WithMessage("A UrlEmail só pode ser preenchida quando o status for 'FinalizadoComSucesso'.");
        }
    }
}