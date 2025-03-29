using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Guards;

namespace Vidsnap.Domain.Entities
{
    public class Video
    {
        private readonly List<VideoStatus> _videoStatuses = [];

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid IdUsuario { get; private set; }
        public string EmailUsuario { get; private set; }
        public string NomeVideo { get; private set; }
        public string Extensao { get; private set; }
        public int Tamanho { get; private set; }
        public int Duracao { get; private set; }
        public DateTime DataInclusao { get; private set; } = DateTime.Now;
        public string? URLZip { get; private set; } = null;
        public string? URLImagem { get; private set; } = null;
        public Status StatusAtual { get; set; } = Status.Recebido;
        public IReadOnlyCollection<VideoStatus> VideoStatuses => _videoStatuses.AsReadOnly();

        protected Video(){}

        public Video(Guid idUsuario, string emailUsuario, string nome, string extensao, int tamanho, int duracao)
        {
            IdUsuario = CommonGuards.AgainstEmptyGuid(idUsuario, nameof(idUsuario));
            EmailUsuario = CommonGuards.AgainstInvalidEmail(emailUsuario, nameof(emailUsuario));
            NomeVideo = CommonGuards.AgainstNullOrWhiteSpace(nome, nameof(nome));
            Extensao = CommonGuards.AgainstNullOrWhiteSpace(extensao, nameof(extensao));
            Tamanho = CommonGuards.AgainstNegativeOrZero(tamanho, nameof(tamanho));
            Duracao = CommonGuards.AgainstNegativeOrZero(duracao, nameof(duracao));

            AtualizarStatus(StatusAtual);
        }

        public void AtualizarStatus(Status status)
        {
            CommonGuards.AgainstEnumOutOfRange(status, nameof(status));

            _videoStatuses.Add(new(status, Id));
            StatusAtual = status;
        }

        public void IncluirURLs(string urlZip, string urlImagem)
        {
            if (!string.IsNullOrEmpty(URLZip))
            {
                throw new InvalidOperationException($"{nameof(URLZip)} já foi definida e não pode ser modificada.");
            }

            if (!string.IsNullOrEmpty(URLImagem))
            {
                throw new InvalidOperationException($"{nameof(URLImagem)} já foi definida e não pode ser modificada.");
            }

            if (StatusAtual != Status.FinalizadoComSucesso)
            {
                throw new InvalidOperationException($"{nameof(URLZip)} e {nameof(URLImagem)} apenas podem ser definidas quando {nameof(StatusAtual)} for igual a {Status.FinalizadoComSucesso}");
            }

            CommonGuards.AgainstNullOrWhiteSpace(urlZip, nameof(urlZip));
            //CommonGuards.AgainstInvalidUrl(urlZip, nameof(urlZip));
            CommonGuards.AgainstNullOrWhiteSpace(urlImagem, nameof(urlImagem));
            //CommonGuards.AgainstInvalidUrl(urlImagem, nameof(urlImagem));

            URLZip = urlZip;
            URLImagem = urlImagem;
        }
    }
}