using AgenticContextEngine.Data.Repositories;
using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<UsuariosIndexDto> ListarAsync()
        {
            var usuarios = await _repository.GetListItemsAsync();
            return new UsuariosIndexDto
            {
                Usuarios = usuarios,
                TotalAdmins = usuarios.Count(u => u.PerfilNome == "Administrador"),
                TotalPerfis = await _repository.CountPerfisAsync()
            };
        }

        public Task<List<OpcaoSelecaoDto>> ObterPerfisAsync() => _repository.GetPerfisAsync();

        public async Task CriarAsync(UsuarioCreateDto dto)
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.SenhaHash),
                PerfilAcessoId = dto.PerfilAcessoId,
                DataCriacao = DateTime.Now
            };
            _repository.Add(usuario);
            await _repository.SaveChangesAsync();
        }

        public async Task<UsuarioEditDto?> ObterParaEdicaoAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return null;

            return new UsuarioEditDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                PerfilAcessoId = usuario.PerfilAcessoId,
                Ativo = usuario.Ativo
            };
        }

        public async Task<OperationResult> EditarAsync(UsuarioEditDto dto)
        {
            var usuario = await _repository.GetByIdAsync(dto.Id);
            if (usuario == null) return OperationResult.Falha("Usuario nao encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
            {
                if (string.IsNullOrEmpty(dto.SenhaAtual) || !BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, usuario.SenhaHash))
                    return OperationResult.Falha("Senha atual incorreta. A senha nao foi alterada.");

                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            }

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.PerfilAcessoId = dto.PerfilAcessoId;
            usuario.Ativo = dto.Ativo;

            await _repository.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task ExcluirAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return;

            _repository.Remove(usuario);
            await _repository.SaveChangesAsync();
        }
    }
}
