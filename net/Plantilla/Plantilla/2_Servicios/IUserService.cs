using Plantilla.Modelos.Entidades;
using Plantilla.Modelos.DTOs;
using Plantilla.Data.Repositorios;
using System;

namespace Plantilla.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserDto userDto);
        Task<User> GetUserByIdAsync(string id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            // Crea el usuario
            return await _userRepository.CreateUserAsync(user, userDto.Password);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

    }
}
