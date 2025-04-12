using FluentValidation;
using Vidsnap.Application.DTOs.Requests;

namespace Vidsnap.Application.DTOs.Validators
{
    public class NovoVideoRequestBodyValidator : AbstractValidator<NovoVideoBodyRequest>
    {
        public NovoVideoRequestBodyValidator()
        {
            //RuleFor(v => v.IdUsuario)
            //    .NotEmpty().WithMessage("O IdUsuario é obrigatório.");

            //RuleFor(v => v.EmailUsuario)
            //    .NotEmpty().WithMessage("O EmailUsuario é obrigatório.")
            //    .EmailAddress().WithMessage("O EmailUsuario deve ser um endereço de e-mail válido.");

            RuleFor(v => v.NomeVideo)
                .NotEmpty().WithMessage("O NomeVideo é obrigatório.")
                .MaximumLength(100).WithMessage("O NomeVideo deve ter no máximo 100 caracteres.");

            RuleFor(v => v.Extensao)
                .NotEmpty().WithMessage("A Extensão do vídeo é obrigatória.")
                .Must(ext => ext != null && new[] { "mp4", "avi", "mov", "mkv", "jpg" }.Contains(ext.ToLower()))
                .WithMessage("A Extensão deve ser um dos formatos suportados: mp4, avi, mov, mkv");

            RuleFor(v => v.Tamanho)
                .GreaterThan(0).WithMessage("O Tamanho do vídeo deve ser maior que zero.");

            RuleFor(v => v.Duracao)
                .GreaterThan(0).WithMessage("A Duração do vídeo deve ser maior que zero.");
        }
    }
}
