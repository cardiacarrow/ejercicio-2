using AsistenciasAPI.Data;
using AsistenciasAPI.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AsistenciasAPI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // Crear usuario con hash de contraseña
        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario, string contraseña)
        {
            usuario.ContraseñaHash = GenerarHashContraseña(contraseña);
            return await _usuarioRepository.AddAsync(usuario);
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            return await _usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario);
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        // Validar usuario y contraseña
        public async Task<bool> ValidarUsuarioAsync(string nombreUsuario, string contraseña)
        {
            var usuario = await _usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario);
            if (usuario == null) return false;

            string hashIngresado = GenerarHashContraseña(contraseña);
            return usuario.ContraseñaHash == hashIngresado;
        }

        // Método para generar hash de la contraseña
        private string GenerarHashContraseña(string contraseña)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
            return Convert.ToBase64String(bytes);
        }
    }
}
