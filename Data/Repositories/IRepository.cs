namespace AgenticContextEngine.Data.Repositories
{
    // Operacoes basicas de acesso a dados, comuns a qualquer entidade com chave int Id.
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        void Add(T entity);
        void Remove(T entity);
        Task SaveChangesAsync();
    }
}
