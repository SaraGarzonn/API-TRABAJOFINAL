using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;
using EmpresaKenxon.Models.Solicitudes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmpresaKenxon.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepositorio usuarioRepositorio;
        private readonly IConfiguration configuracion;

        // ASP.NET Core nos entrega el repositorio y la configuración (appsettings.json).
        public AuthController(IUsuarioRepositorio usuarioRepositorio, IConfiguration configuracion)
        {
            this.usuarioRepositorio = usuarioRepositorio;
            this.configuracion = configuracion;
        }

        // POST /api/auth/login – público (no lleva [Authorize])
        [HttpPost("login")]
        public IActionResult Login(LoginSolicitud solicitud)
        {
            Usuario usuario = usuarioRepositorio.ObtenerPorNombreUsuario(solicitud.NombreUsuario);

            if (usuario == null || !usuario.ValidarContrasena(solicitud.Contrasena))
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            string token = GenerarToken(usuario);
            return Ok(new { token = token, nombreUsuario = usuario.NombreUsuario, rol = usuario.Rol.ToString() });
        }

        // Arma el token JWT con tres datos (claims): Id, NombreUsuario y Rol.
        private string GenerarToken(Usuario usuario)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("NombreUsuario", usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            // La clave secreta "firma" el token para que nadie lo pueda falsificar.
            SymmetricSecurityKey clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracion["Jwt:Clave"]));
            SigningCredentials credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuracion["Jwt:Emisor"],
                audience: configuracion["Jwt:Audiencia"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
