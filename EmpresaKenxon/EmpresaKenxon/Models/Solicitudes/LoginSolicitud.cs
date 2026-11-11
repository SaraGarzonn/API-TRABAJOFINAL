namespace EmpresaKenxon.Models.Solicitudes
{
    // Datos que llegan en POST /api/auth/login
    public class LoginSolicitud
    {
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
    }
}
