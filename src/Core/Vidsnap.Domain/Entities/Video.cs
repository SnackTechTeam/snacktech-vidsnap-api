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
        public string Nome { get; private set; }
        public string Extensao { get; private set; }
        public int Tamanho { get; private set; }
        public int Duracao { get; private set; }
        public DateTime DataInclusao { get; private set; } = DateTime.Now;
        public string? URLZipe { get; private set; } = null;
        public Status StatusAtual { get; set; } = Status.Recebido;
        public IReadOnlyCollection<VideoStatus> VideoStatuses => _videoStatuses.AsReadOnly();

        protected Video(){}

        public Video(Guid idUsuario, string emailUsuario, string nome, string extensao, int tamanho, int duracao)
        {
            IdUsuario = CommonGuards.AgainstEmptyGuid(idUsuario, nameof(idUsuario));
            EmailUsuario = CommonGuards.AgainstInvalidEmail(emailUsuario, nameof(emailUsuario));
            Nome = CommonGuards.AgainstNullOrWhiteSpace(nome, nameof(nome));
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

        public void IncluirURLZipe(string urlZipe)
        {
            if (!string.IsNullOrEmpty(URLZipe))
            {
                throw new InvalidOperationException($"{nameof(URLZipe)} já foi definida e não pode ser modificada.");
            }

            if (StatusAtual != Status.FinalizadoComSucesso)
            {
                throw new InvalidOperationException($"{nameof(URLZipe)} apenas pode ser definida quando {nameof(StatusAtual)} for igual a {Status.FinalizadoComSucesso}");
            }

            CommonGuards.AgainstNullOrWhiteSpace(urlZipe, nameof(urlZipe));
            CommonGuards.AgainstInvalidUrl(urlZipe, nameof(urlZipe));

            URLZipe = urlZipe;
        }
    }
}