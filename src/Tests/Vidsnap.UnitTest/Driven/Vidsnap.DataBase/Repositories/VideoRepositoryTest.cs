using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vidsnap.DataBase.Repositories;
using Vidsnap.DataBase.Context;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;

namespace Vidsnap.UnitTest.Driven.Vidsnap.DataBase.Repositories
{
    public class VideoRepositoryTest
    {
        #region SalvarAsync

        [Fact]
        public async Task CriarAsync_QuandoVideoForValido_DeveSalvarComSucesso()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();

            //act
            await repository.CriarAsync(video);

            //assert
            Assert.Equal(1, await appDbContext.Videos.CountAsync());
            Assert.True(await appDbContext.Videos.AnyAsync(v => v.NomeVideo == "video"));
        }

        [Fact]
        public async Task CriarAsync_QuandoVideoForNulo_DeveLancarException()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);            

            //act
            Func<Task> act = async () => await repository.CriarAsync(null!);

            //assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_QuandoIdExistir_DeveRetornarVideo()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();
            await appDbContext.Videos.AddAsync(video);
            await appDbContext.SaveChangesAsync();

            //act
            var resultado = await repository.ObterPorIdAsync(video.Id);

            //assert
            resultado.Should().NotBeNull();
            resultado.NomeVideo.Should().Be(video.NomeVideo);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoIdNaoExistir_DeveRetornarNull()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();
            await appDbContext.Videos.AddAsync(video);
            await appDbContext.SaveChangesAsync();

            //act
            var resultado = await repository.ObterPorIdAsync(Guid.NewGuid());

            //assert
            resultado.Should().BeNull();            
        }

        #endregion

        #region ObterTodosDoUsuario

        [Fact]
        public async Task ObterTodosDoUsuarioAsync_QuandoUsuarioTiverVideos_DeveRetornarSuaLista()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var idUsuario = Guid.NewGuid();
            List<Video> videos = [
                new Video(idUsuario, "email@email.com.br", "video1", "abc", 1, 1),
                new Video(idUsuario, "email@email.com.br", "video2", "abc", 1, 1),
                new Video(Guid.NewGuid(), "outro_email@email.com.br", "outro_video", "abc", 1, 1),
            ];

            await appDbContext.Videos.AddRangeAsync(videos);
            await appDbContext.SaveChangesAsync();

            //act
            var resultado = await repository.ObterTodosDoUsuarioAsync(idUsuario);

            //assert
            resultado.Should().NotBeEmpty();
            resultado.Count.Should().Be(2);
        }

        #endregion

        #region AtualizarStatusProcessamentoAsync

        [Fact]
        public async Task AtualizarStatusProcessamentoAsync_QuandoForStatusNovo_DeveAtualizarStatus()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();
            await appDbContext.Videos.AddAsync(video);
            await appDbContext.SaveChangesAsync();

            appDbContext.Entry(video).State = EntityState.Detached;

            video = await appDbContext.Videos.AsNoTracking().FirstAsync(v => v.Id == video.Id);

            video.AtualizarStatus(Status.Processando);

            //act
            var resultado = await repository.AtualizarStatusProcessamentoAsync(video, Status.Processando);

            //assert
            var videoAlterado = await appDbContext.Videos
                .Include(v => v.VideoStatuses)
                .FirstAsync(v => v.Id == video.Id);

            resultado.Should().Be(2);//Deve ser alterado Video e adicionado VideoStatus
            videoAlterado.StatusAtual.Should().Be(Status.Processando);
            videoAlterado.VideoStatuses.Any(vs => vs.Status == Status.Processando).Should().BeTrue();
        }

        [Fact]
        public async Task AtualizarStatusProcessamentoAsync_QuandoTemUrls_DeveAtualizarUrl()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();
            await appDbContext.Videos.AddAsync(video);
            await appDbContext.SaveChangesAsync();

            appDbContext.Entry(video).State = EntityState.Detached;

            video = await appDbContext.Videos.AsNoTracking().FirstAsync(v => v.Id == video.Id);

            video.AtualizarStatus(Status.FinalizadoComSucesso);

            var urlZip = "http://teste/video.mp4";
            var urlImagem = "http://teste/imagem.jpg";
            video.IncluirURLs(urlZip, urlImagem);

            //act
            var resultado = await repository.AtualizarStatusProcessamentoAsync(video, Status.FinalizadoComSucesso);

            //assert
            var videoAlterado = await appDbContext.Videos
                .Include(v => v.VideoStatuses)
                .FirstAsync(v => v.Id == video.Id);

            resultado.Should().Be(2);//Deve ser alterado Video e adicionado VideoStatus
            videoAlterado.StatusAtual.Should().Be(Status.FinalizadoComSucesso);
            videoAlterado.VideoStatuses.Any(vs => vs.Status == Status.FinalizadoComSucesso).Should().BeTrue();
            videoAlterado.URLZip.Should().Be(urlZip);
        }

        [Fact]
        public async Task AtualizarStatusProcessamentoAsync_QuandoVideoNaoExistir_DeveLancarException()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();

            video.AtualizarStatus(Status.Processando);

            //act
            Func<Task> act = async () => await repository.AtualizarStatusProcessamentoAsync(video, Status.Processando);

            //assert
            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task AtualizarStatusProcessamentoAsync_QuandoStatusNaoExistirNaEntidade_DeveLancarException()
        {
            //arrange
            var options = CriarOpcoesEmMemoria();

            using var appDbContext = new AppDbContext(options);
            var repository = new VideoRepository(appDbContext);

            var video = CriarVideoValido();

            //act
            Func<Task> act = async () => await repository.AtualizarStatusProcessamentoAsync(video, Status.Processando);

            //assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
        #endregion

        #region Private Methods

        private static DbContextOptions<AppDbContext> CriarOpcoesEmMemoria()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Garante um BD limpo a cada teste
                .Options;
        }

        private static Video CriarVideoValido()
        {
            return new(Guid.NewGuid(), 
                "email@email.com.br", 
                "video", 
                "abc", 
                1, 
                1);
        }

        #endregion
    }
}
