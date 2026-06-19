using AgenticContextEngine.Data.Repositories;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public class AgenteService : IAgenteService
    {
        private readonly IAgenteRepository _repository;

        public AgenteService(IAgenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<AgentesIndexDto> ListarAsync()
        {
            return new AgentesIndexDto
            {
                Agentes = await _repository.GetListItemsAsync(),
                TotalMensagens = await _repository.CountMensagensAsync(),
                TotalMemorias = await _repository.CountMemoriasAsync()
            };
        }

        public Task<List<OpcaoSelecaoDto>> ObterCategoriasAsync() => _repository.GetCategoriasAsync();

        public async Task CriarAsync(AgenteFormDto dto, int? criadoPorUsuarioId)
        {
            var agente = new Agente
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CategoriaAgenteId = dto.CategoriaAgenteId,
                Instrucoes = dto.Instrucoes,
                Ativo = dto.Ativo,
                DataCriacao = DateTime.Now,
                CriadoPorUsuarioId = criadoPorUsuarioId
            };
            _repository.Add(agente);
            await _repository.SaveChangesAsync();
        }

        public async Task<OperationResult<AgenteFormDto>> ObterParaEdicaoAsync(int id, int? usuarioId, bool isAdmin)
        {
            var agente = await _repository.GetByIdAsync(id);
            if (agente == null) return OperationResult<AgenteFormDto>.Falha("Agente nao encontrado.");

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, agente.CriadoPorUsuarioId))
                return OperationResult<AgenteFormDto>.Falha("Voce so pode editar agentes criados por voce.");

            return OperationResult<AgenteFormDto>.Ok(new AgenteFormDto
            {
                Id = agente.Id,
                Nome = agente.Nome,
                CategoriaAgenteId = agente.CategoriaAgenteId,
                Descricao = agente.Descricao,
                Instrucoes = agente.Instrucoes,
                Ativo = agente.Ativo
            });
        }

        public async Task<OperationResult> EditarAsync(AgenteFormDto dto, int? usuarioId, bool isAdmin)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null) return OperationResult.Falha("Agente nao encontrado.");

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, existente.CriadoPorUsuarioId))
                return OperationResult.Falha("Voce so pode editar agentes criados por voce.");

            existente.Nome = dto.Nome;
            existente.Descricao = dto.Descricao;
            existente.CategoriaAgenteId = dto.CategoriaAgenteId;
            existente.Instrucoes = dto.Instrucoes;
            existente.Ativo = dto.Ativo;

            await _repository.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult<bool>> ExcluirAsync(int id, int? usuarioId, bool isAdmin)
        {
            var agente = await _repository.GetByIdAsync(id);
            if (agente == null) return OperationResult<bool>.Ok(false);

            if (!AuthHelper.PodeGerenciar(isAdmin, usuarioId, agente.CriadoPorUsuarioId))
                return OperationResult<bool>.Falha("Voce so pode excluir agentes criados por voce.");

            if (await _repository.TemHistoricoVinculadoAsync(id))
                return OperationResult<bool>.Falha("Nao foi possivel excluir este agente pois ele possui historico vinculado.");

            _repository.Remove(agente);
            await _repository.SaveChangesAsync();
            return OperationResult<bool>.Ok(true);
        }
    }
}
