using Plantilla.Modelos.Entidades;
using Plantilla.Modelos.DTOs;
using Plantilla.Data.Repositorios;
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
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager; 

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager; 
        }

        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            // Verifica si el email ya está registrado
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
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
            return await _userRepository.CreateUserAsync(user, userDto.Password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        
        public async Task<User> AuthenticateAsync(string email, string password)
        {
            // Obtiene el usuario por email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null; 
            }

            // Verifica la contraseña
            var passwordHasher = new PasswordHasher<User>(); 
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return null; 
            }

            return user; 
        }
    }
}
