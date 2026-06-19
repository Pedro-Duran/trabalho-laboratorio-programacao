using AgenticContextEngine.Data.Repositories;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public Task<List<CategoriaListItemDto>> ListarAsync() => _repository.GetListItemsAsync();

        public async Task CriarAsync(CategoriaFormDto dto, int? criadoPorUsuarioId)
        {
            var categoria = new CategoriaAgente
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Ativo = dto.Ativo,
                CriadoPorUsuarioId = criadoPorUsuarioId
            };
            _repository.Add(categoria);
            await _repository.SaveChangesAsync();
        }

        public async Task<OperationResult<CategoriaFormDto>> ObterParaEdicaoAsync(int id, int? usuarioId, bool isAdmin)
        {
            var categoria = await _repository.GetByIdAsync(id);
            if (categoria == null) return OperationResult<CategoriaFormDto>.Falha("Categoria nao encontrada.");

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, categoria.CriadoPorUsuarioId))
                return OperationResult<CategoriaFormDto>.Falha("Voce so pode editar categorias criadas por voce.");

            return OperationResult<CategoriaFormDto>.Ok(new CategoriaFormDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao,
                Ativo = categoria.Ativo
            });
        }

        public async Task<OperationResult> EditarAsync(CategoriaFormDto dto, int? usuarioId, bool isAdmin)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null) return OperationResult.Falha("Categoria nao encontrada.");

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, existente.CriadoPorUsuarioId))
                return OperationResult.Falha("Voce so pode editar categorias criadas por voce.");

            existente.Nome = dto.Nome;
            existente.Descricao = dto.Descricao;
            existente.Ativo = dto.Ativo;

            await _repository.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult<bool>> ExcluirAsync(int id, int? usuarioId, bool isAdmin)
        {
            var categoria = await _repository.GetByIdAsync(id);
            if (categoria == null) return OperationResult<bool>.Ok(false);

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, categoria.CriadoPorUsuarioId))
                return OperationResult<bool>.Falha("Voce so pode excluir categorias criadas por voce.");

            if (await _repository.TemAgentesVinculadosAsync(id))
                return OperationResult<bool>.Falha("Nao foi possivel excluir esta categoria pois ela possui agentes vinculados.");

            _repository.Remove(categoria);
            await _repository.SaveChangesAsync();
            return OperationResult<bool>.Ok(true);
        }
    }
}
