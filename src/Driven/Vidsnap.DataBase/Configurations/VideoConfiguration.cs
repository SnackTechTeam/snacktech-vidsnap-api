using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using Vidsnap.Domain.Entities;

namespace Vidsnap.DataBase.Configurations
{
    [ExcludeFromCodeCoverage]
    internal sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable(nameof(Video));

            builder.HasKey(v => v.Id);

            builder.Property(v => v.IdUsuario)
                .IsRequired();

            builder.Property(v => v.EmailUsuario)
                .HasColumnType("varchar")
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(v => v.NomeVideo)
                .HasColumnType("varchar")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.Extensao)
                .HasColumnType("varchar")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(v => v.Tamanho)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(v => v.Duracao)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(v => v.DataInclusao)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(v => v.URLZip)
                .HasColumnType("varchar")
                .HasMaxLength(500);

            builder.Property(v => v.URLImagem)
                .HasColumnType("varchar")
                .HasMaxLength(500);

            builder.HasMany(v => v.VideoStatuses)
                .WithOne(vs => vs.Video)
                .HasForeignKey(v => v.IdVideo);

            builder.Navigation(nameof(Video.VideoStatuses))
                .HasField("_videoStatuses");
        }
    }
}
