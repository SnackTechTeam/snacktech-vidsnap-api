using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Guards;

namespace Vidsnap.Domain.Entities
{
    public class VideoStatus(Status status, Guid idVideo)
    {
        public Status Status { get; private set; } = CommonGuards.AgainstEnumOutOfRange(status, nameof(status));
        public Guid IdVideo { get; private set; } = CommonGuards.AgainstEmptyGuid(idVideo, nameof(idVideo));
        public Video? Video { get; private set; } = null;
        public DateTime DataInclusao { get; private set; } = DateTime.Now;
    }
}