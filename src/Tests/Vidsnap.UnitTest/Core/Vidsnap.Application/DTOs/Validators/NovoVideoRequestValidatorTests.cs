using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Validators;
using FluentValidation.TestHelper;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.DTOs.Validators
{
    public class NovoVideoRequestValidatorTests
    {
        private readonly NovoVideoRequestValidator _validator;

        public NovoVideoRequestValidatorTests()
        {
            _validator = new NovoVideoRequestValidator();
        }

        [Fact]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoIdUsuarioForVazio()
        {
            var request = new NovoVideoRequest(Guid.Empty, "teste@teste.com", "teste", "mp4", 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.IdUsuario).WithErrorMessage("O IdUsuario é obrigatório.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoEmailUsuarioForInvalido(string? email)
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), email, "teste", "mp4", 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.EmailUsuario).WithErrorMessage("O EmailUsuario é obrigatório.");
        }

        [Fact]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoEmailUsuarioNaoForValido()
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "email-invalido", "teste", "mp4", 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.EmailUsuario).WithErrorMessage("O EmailUsuario deve ser um endereço de e-mail válido.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoNomeVideoForVazio(string? nome)
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", nome, "mp4", 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.NomeVideo).WithErrorMessage("O NomeVideo é obrigatório.");
        }

        [Fact]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoNomeVideoForMaiorQue100Caracteres()
        {            
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", new string('A', 101), "mp4", 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.NomeVideo).WithErrorMessage("O NomeVideo deve ter no máximo 100 caracteres.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("exe")]
        [InlineData("txt")]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoExtensaoForInvalida(string? extensao)
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "teste", extensao, 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.Extensao);
        }

        [Theory]
        [InlineData("mp4")]
        [InlineData("avi")]
        [InlineData("mov")]
        [InlineData("mkv")]
        public void Construtor_NovoVideoRequestValidator_DevePassar_QuandoExtensaoForValida(string extensao)
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "teste", extensao, 1, 1);
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(r => r.Extensao);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoTamanhoForMenorOuIgualAZero(int tamanho)
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "teste", "mp4", tamanho, 1);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.Tamanho).WithErrorMessage("O Tamanho do vídeo deve ser maior que zero.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Construtor_NovoVideoRequestValidator_DeveFalhar_QuandoDuracaoForMenorOuIgualAZero(int duracao)
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "teste", "mp4", 1, duracao);
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(r => r.Duracao).WithErrorMessage("A Duração do vídeo deve ser maior que zero.");
        }

        [Fact]
        public void Construtor_NovoVideoRequestValidator_DevePassar_QuandoTodosOsCamposForemValidos()
        {
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "teste", "mp4", 1, 1);

            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
