using AsistenciasAPI.Models;

namespace AsistenciasAPI.Data
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario> AddAsync(Usuario usuario);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
    }
}
