using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidsnap.Application.DTOs.Requests;

namespace Vidsnap.Application.DTOs.Validators
{
    public class UrlPreAssindaRequestValidator : AbstractValidator<UrlPreAssinadaRequest>
    {
        public UrlPreAssindaRequestValidator()
        {
            RuleFor(v => v.IdUsuario)
                .NotEmpty().WithMessage("O IdUsuario é obrigatório.");

            RuleFor(v => v.NomeArquivo)
                .NotEmpty().WithMessage("O NomeArquivo é obrigatório.")
                .MaximumLength(100).WithMessage("O NomeArquivo deve ter no máximo 100 caracteres.");
        }
    }
}
