using AgenticContextEngine.Data.Repositories;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public class CanalService : ICanalService
    {
        private readonly ICanalRepository _repository;

        public CanalService(ICanalRepository repository)
        {
            _repository = repository;
        }

        public async Task<CanaisIndexDto> ListarAsync()
        {
            return new CanaisIndexDto
            {
                Canais = await _repository.GetListItemsAsync(),
                TotalSessoes = await _repository.CountSessoesAsync()
            };
        }

        public async Task CriarAsync(CanalFormDto dto, int? criadoPorUsuarioId)
        {
            var canal = new CanalOrigem
            {
                Nome = dto.Nome,
                UrlSite = dto.UrlSite,
                Descricao = dto.Descricao,
                Ativo = dto.Ativo,
                DataCriacao = DateTime.Now,
                CriadoPorUsuarioId = criadoPorUsuarioId
            };
            _repository.Add(canal);
            await _repository.SaveChangesAsync();
        }

        public async Task<OperationResult<CanalFormDto>> ObterParaEdicaoAsync(int id, int? usuarioId, bool isAdmin)
        {
            var canal = await _repository.GetByIdAsync(id);
            if (canal == null) return OperationResult<CanalFormDto>.Falha("Canal nao encontrado.");

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, canal.CriadoPorUsuarioId))
                return OperationResult<CanalFormDto>.Falha("Voce so pode editar canais criados por voce.");

            return OperationResult<CanalFormDto>.Ok(new CanalFormDto
            {
                Id = canal.Id,
                Nome = canal.Nome,
                UrlSite = canal.UrlSite,
                Descricao = canal.Descricao,
                Ativo = canal.Ativo
            });
        }

        public async Task<OperationResult> EditarAsync(CanalFormDto dto, int? usuarioId, bool isAdmin)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null) return OperationResult.Falha("Canal nao encontrado.");

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, existente.CriadoPorUsuarioId))
                return OperationResult.Falha("Voce so pode editar canais criados por voce.");

            existente.Nome = dto.Nome;
            existente.UrlSite = dto.UrlSite;
            existente.Descricao = dto.Descricao;
            existente.Ativo = dto.Ativo;

            await _repository.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult<bool>> ExcluirAsync(int id, int? usuarioId, bool isAdmin)
        {
            var canal = await _repository.GetByIdAsync(id);
            if (canal == null) return OperationResult<bool>.Ok(false);

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, canal.CriadoPorUsuarioId))
                return OperationResult<bool>.Falha("Voce so pode excluir canais criados por voce.");

            if (await _repository.TemSessoesVinculadasAsync(id))
                return OperationResult<bool>.Falha("Nao foi possivel excluir este canal pois ele possui sessoes vinculadas.");

            _repository.Remove(canal);
            await _repository.SaveChangesAsync();
            return OperationResult<bool>.Ok(true);
        }
    }
}
