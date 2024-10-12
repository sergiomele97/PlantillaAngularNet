using Plantilla.Modelos.DTOs;
using Plantilla.Modelos.Entidades;

namespace Plantilla.Services
{
    public static class UserMapper
    {
        // Método para convertir de User a UserDto
        public static UserDto ToDto(User user)
        {
            if (user == null)
                return null;

            return new UserDto
            {
                Email = user.Email,
                // Aquí puedes agregar otros campos si los tienes
                // La contraseña NO debe ser incluida por razones de seguridad
            };
        }

        // Método para convertir de UserDto a User
        public static User ToEntity(UserDto userDto)
        {
            if (userDto == null)
                return null;

            return new User
            {
                Email = userDto.Email,
                // No incluyas la contraseña aquí ya que se manejará por UserManager
            };
        }
    }
}
