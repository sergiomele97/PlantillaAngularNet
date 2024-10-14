using Plantilla.Modelos.Entidades;
using Plantilla.Modelos.DTOs;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Plantilla.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserDto userDto);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AuthenticateAsync(string email, string password);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            // Verifica si el email ya está registrado
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("El email ya está registrado.");
            }

            // Utiliza el email como UserName
            var user = new User
            {
                UserName = userDto.Email,
                Email = userDto.Email
            };

            // Crea el usuario con la contraseña
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                // Extraer los mensajes de error de la lista de errores
                var errorMessages = result.Errors.Select(error => error.Description);

                // Lanzar una excepción con los mensajes de error
                throw new ArgumentException("Error al crear el usuario: " + string.Join(", ", errorMessages));
            }

            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            // Usa UserManager para obtener el usuario por email
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Si el inicio de sesión fue exitoso, busca el usuario
                var user = await _userManager.FindByEmailAsync(email);
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
