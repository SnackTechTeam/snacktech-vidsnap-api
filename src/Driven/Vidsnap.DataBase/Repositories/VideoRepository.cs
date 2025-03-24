using Microsoft.EntityFrameworkCore;
using Vidsnap.DataBase.Context;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.DataBase.Repositories
{
    public class VideoRepository(AppDbContext appDbContext) : IVideoRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        public async Task CriarAsync(Video video)
        {
            await _appDbContext.Videos.AddAsync(video);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Video?> ObterPorIdAsync(Guid idVideo)
        {
            return await _appDbContext.Videos.
                AsNoTracking().
                FirstOrDefaultAsync(v => v.Id == idVideo);
        }

        public async Task<IReadOnlyList<Video>> ObterTodosDoUsuarioAsync(Guid idUsuario)
        {
            return await _appDbContext.Videos
                    .AsNoTracking()
                    .Include(v => v.VideoStatuses)
                    .Where(v => v.IdUsuario == idUsuario)
                    .ToListAsync();
        }        

        public async Task<int> AtualizarStatusProcessamentoAsync(Video video, Status newStatus)
        {
            _appDbContext.Attach(video);

            var videoEntry = _appDbContext.Entry(video);
            videoEntry.Property(v => v.StatusAtual).IsModified = true;
            if (video.URLZip is not null)
            {
                videoEntry.Property(v => v.URLZip).IsModified = true;
            }

            _appDbContext.Entry(video.VideoStatuses.First(vs => vs.Status == newStatus)).State = EntityState.Added;
            
            var entriesUpdated = await _appDbContext.SaveChangesAsync();
            return entriesUpdated;
        }        
    }
}