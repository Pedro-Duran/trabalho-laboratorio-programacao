using Microsoft.EntityFrameworkCore;

namespace AgenticContextEngine.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> Set;

        public Repository(AppDbContext context)
        {
            Context = context;
            Set = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync() => await Set.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);

        public void Add(T entity) => Set.Add(entity);

        public void Remove(T entity) => Set.Remove(entity);

        public async Task SaveChangesAsync() => await Context.SaveChangesAsync();
    }
}
