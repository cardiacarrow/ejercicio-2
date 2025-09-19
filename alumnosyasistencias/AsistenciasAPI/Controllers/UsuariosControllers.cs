using AsistenciasAPI.Models;
using AsistenciasAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AsistenciasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;

        public UsuariosController(IUsuarioService usuarioService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        // ===========================
        // Registro de usuario
        // ===========================
        [HttpPost("register")]
        [AllowAnonymous] // ✅ Permitir acceso sin token
        public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Contraseña))
                return BadRequest(new { mensaje = "La contraseña es obligatoria" });

            var usuarioExistente = await _usuarioService.ObtenerPorNombreUsuarioAsync(request.NombreUsuario);
            if (usuarioExistente != null)
                return BadRequest(new { mensaje = "El nombre de usuario ya existe" });

            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                NombreCompleto = request.NombreCompleto,
                Rol = request.Rol
            };

            var nuevoUsuario = await _usuarioService.CrearUsuarioAsync(usuario, request.Contraseña);

            return Ok(new
            {
                Id = nuevoUsuario.Id,
                nuevoUsuario.NombreUsuario,
                nuevoUsuario.NombreCompleto,
                nuevoUsuario.Rol
            });
        }

        // ===========================
        // Login / Autenticación
        // ===========================
        [HttpPost("login")]
        [AllowAnonymous] // ✅ Permitir acceso sin token
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var usuarioValido = await _usuarioService.ValidarUsuarioAsync(login.NombreUsuario, login.Contraseña);
            if (!usuarioValido)
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            var usuario = await _usuarioService.ObtenerPorNombreUsuarioAsync(login.NombreUsuario);
            if (usuario == null)
                return Unauthorized();

            var token = GenerarToken(usuario);

            return Ok(new
            {
                usuario.Id,
                usuario.NombreUsuario,
                usuario.NombreCompleto,
                usuario.Rol,
                Token = token
            });
        }

        // ===========================
        // Endpoint protegido de prueba
        // ===========================
        [Authorize(Roles = "Docente")] 
        [HttpGet("protegido")]
        public IActionResult Protegido()
        {
            return Ok(new { mensaje = "Solo los docentes pueden ver esto" });
        }

        [Authorize] // Cualquier usuario autenticado puede acceder
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();

            // Mapear a un DTO seguro sin ContraseñaHash
            var usuariosSeguros = usuarios.Select(u => new 
            {
                u.Id,
                u.NombreUsuario,
                u.NombreCompleto,
                u.Rol
            }).ToList();

            return Ok(usuariosSeguros);
        }


        // ===========================
        // Método para generar JWT
        // ===========================
        private string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NombreUsuario),
                new Claim("id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ===========================
        // DTOs
        // ===========================
        public class LoginRequest
        {
            public string NombreUsuario { get; set; } = string.Empty;
            public string Contraseña { get; set; } = string.Empty;
        }

        public class RegistroRequest
        {
            public string NombreUsuario { get; set; } = string.Empty;
            public string NombreCompleto { get; set; } = string.Empty;
            public string Rol { get; set; } = "Docente"; // ✅ Rol por defecto
            public string Contraseña { get; set; } = string.Empty;
        }
    }
}
