using AsistenciasAPI.Models;

namespace AsistenciasAPI.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> CrearUsuarioAsync(Usuario usuario, string contraseña);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task<List<Usuario>> ObtenerTodosAsync();
        Task<bool> ValidarUsuarioAsync(string nombreUsuario, string contraseña);
    }
}
