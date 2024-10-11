// net\Plantilla\Plantilla\Data\Repositorios\IUserRepository.cs
using Microsoft.AspNetCore.Identity;
using Plantilla.Data.BBDD.Contexto;
using Plantilla.Modelos.Entidades;

namespace Plantilla.Data.Repositorios
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user, string password);
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email); // Agregar este método
    }

    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return user;
            }
            else
            {
                // Manejo de errores: lanzar una excepción o retornar un error específico
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            // Buscar el usuario por email
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
