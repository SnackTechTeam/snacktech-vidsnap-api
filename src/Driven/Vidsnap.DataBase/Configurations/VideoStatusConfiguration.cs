using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vidsnap.Domain.Entities;

namespace Vidsnap.DataBase.Configurations
{
    internal sealed class VideoStatusConfiguration : IEntityTypeConfiguration<VideoStatus>
    {
        public void Configure(EntityTypeBuilder<VideoStatus> builder)
        {
            builder.ToTable(nameof(VideoStatus));

            builder.HasKey(vs => new { vs.VideoId, vs.Status });

            builder.Property(vs => vs.Status)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(vs => vs.DataInclusao)
                .HasColumnType("datetime")
                .IsRequired();
        }
    }
}
