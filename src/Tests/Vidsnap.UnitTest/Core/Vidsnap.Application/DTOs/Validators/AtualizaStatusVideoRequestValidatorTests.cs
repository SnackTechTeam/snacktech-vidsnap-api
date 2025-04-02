using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Validators;
using Vidsnap.Domain.Enums;
using FluentValidation.TestHelper;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.DTOs.Validators
{
    public class AtualizaStatusVideoRequestValidatorTests
    {
        private readonly AtualizaStatusVideoRequestValidator _validator;

        public AtualizaStatusVideoRequestValidatorTests()
        {
            _validator = new AtualizaStatusVideoRequestValidator();
        }

        [Fact]
        public void Construtor_AtualizaStatusVideoRequestValidator_DeveFalhar_QuandoIdVideoForVazio()
        {
            var request = new AtualizaStatusVideoRequest(Guid.Empty, Status.Processando.ToString());
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.IdVideo)
                  .WithErrorMessage("O Id do vídeo é obrigatório.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Construtor_AtualizaStatusVideoRequestValidator_DeveFalhar_QuandoStatusForVazioOuNulo(string? status)
        {
            var request = new AtualizaStatusVideoRequest(Guid.NewGuid(), status);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.Status)
                  .WithErrorMessage("O Status é obrigatório.");
        }

        [Theory]
        [InlineData("Invalido")]
        [InlineData("123")]
        [InlineData("OutroStatus")]
        public void Construtor_AtualizaStatusVideoRequestValidator_DeveFalhar_QuandoStatusForInvalido(string status)
        {
            var request = new AtualizaStatusVideoRequest(Guid.NewGuid(), status);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.Status)
                  .WithErrorMessage("O Status informado é inválido.");
        }

        [Fact]
        public void Construtor_AtualizaStatusVideoRequestValidator_DeveFalhar_QuandoUrlZipForPreenchidaESemStatusFinalizado()
        {
            var request = new AtualizaStatusVideoRequest(Guid.NewGuid(), Status.Processando.ToString(), "http://example.com/video.zip");
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.UrlZip)
                  .WithErrorMessage("A UrlZip só pode ser preenchida quando o status for 'FinalizadoComSucesso'.");
        }

        [Fact]
        public void Construtor_AtualizaStatusVideoRequestValidator_DeveFalhar_QuandoUrlImagemForPreenchidaESemStatusFinalizado()
        {
            var request = new AtualizaStatusVideoRequest(Guid.NewGuid(), Status.Processando.ToString(), null, "http://example.com/video.jpg");
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.UrlImagem)
                  .WithErrorMessage("A UrlEmail só pode ser preenchida quando o status for 'FinalizadoComSucesso'.");
        }

        [Fact]
        public void Construtor_AtualizaStatusVideoRequestValidator_DevePassar_QuandoTodosOsCamposForemValidos()
        {
            var request = new AtualizaStatusVideoRequest(Guid.NewGuid(), Status.FinalizadoComSucesso.ToString(), "http://example.com/video.zip", "http://example.com/video.jpg");

            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
