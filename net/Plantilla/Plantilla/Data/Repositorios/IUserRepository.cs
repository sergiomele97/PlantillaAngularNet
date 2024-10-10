using Plantilla.Data.Contextos;
using Plantilla.Data.Entidades;

namespace Plantilla.Data.Repositorios
{
    // 1. Definimos la interfaz IUserRepository
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        // Add other repository methods as needed
    }

    // 2. Implementamos la interfaz
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Add other repository methods as needed
    }
}
