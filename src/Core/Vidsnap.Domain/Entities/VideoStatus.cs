using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Guards;

namespace Vidsnap.Domain.Entities
{
    public class VideoStatus(Status status, Guid videoId)
    {
        public Status Status { get; private set; } = CommonGuards.AgainstEnumOutOfRange(status, nameof(status));
        public Guid VideoId { get; private set; } = CommonGuards.AgainstEmptyGuid(videoId, nameof(videoId));
        public Video? Video { get; private set; } = null;
        public DateTime DataInclusao { get; private set; } = DateTime.Now;
    }
}