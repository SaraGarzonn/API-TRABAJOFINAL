using EmpresaKenxon.Controllers;
using EmpresaKenxon.Models.Solicitudes;
using EmpresaKenxon.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EmpresaKenxon.Pruebas
{
    
    // Dispose() a ahí borramos la carpeta temporal
    public class LoginPruebas : IDisposable
    {
        private readonly string carpetaTemporal; //carpeta temporal que inventa un codigo para cada prueba
        private readonly AuthController controlador;

        public LoginPruebas()
        {
            carpetaTemporal = Path.Combine(Path.GetTempPath(), "KenxonPruebas_" + Guid.NewGuid());
            Directory.CreateDirectory(carpetaTemporal);

            // Como el archivo no existe, el repositorio crea los 3 usuarios del sistema.
            UsuarioRepositorio repositorio = new UsuarioRepositorio(Path.Combine(carpetaTemporal, "usuarios.json"));

            // Configuración de prueba
            IConfiguration configuracion = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Clave", "ClaveSecretaDePruebasKenxonCarSound2026" },
                    { "Jwt:Emisor", "EmpresaKenxon" },
                    { "Jwt:Audiencia", "EmpresaKenxonUsuarios" }
                })
                .Build();

            controlador = new AuthController(repositorio, configuracion);
        }

        public void Dispose()
        {
            Directory.Delete(carpetaTemporal, true);
        }

        [Fact]
        public void Login_CredencialesCorrectas_DevuelveToken()
        {
            LoginSolicitud solicitud = new LoginSolicitud { NombreUsuario = "secretaria", Contrasena = "Secretaria123*" };

            IActionResult resultado = controlador.Login(solicitud);

            Assert.IsType<OkObjectResult>(resultado); // 200
        }

        [Fact]
        public void Login_ContrasenaIncorrecta_DevuelveNoAutorizado()
        {
            LoginSolicitud solicitud = new LoginSolicitud { NombreUsuario = "jefe", Contrasena = "ClaveEquivocada" };

            IActionResult resultado = controlador.Login(solicitud);

            Assert.IsType<UnauthorizedObjectResult>(resultado); // 401
        }

        [Fact]
        public void Login_UsuarioInexistente_DevuelveNoAutorizado()
        {
            LoginSolicitud solicitud = new LoginSolicitud { NombreUsuario = "gerente", Contrasena = "Jefe123*" };

            IActionResult resultado = controlador.Login(solicitud);

            Assert.IsType<UnauthorizedObjectResult>(resultado); // 401
        }
    }
}
