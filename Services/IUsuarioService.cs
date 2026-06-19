using AgenticContextEngine.Models;

namespace AgenticContextEngine.Services
{
    public interface IUsuarioService
    {
        Task<UsuariosIndexDto> ListarAsync();
        Task<List<OpcaoSelecaoDto>> ObterPerfisAsync();
        Task CriarAsync(UsuarioCreateDto dto);
        Task<UsuarioEditDto?> ObterParaEdicaoAsync(int id);
        Task<OperationResult> EditarAsync(UsuarioEditDto dto);
        Task ExcluirAsync(int id);
    }
}
