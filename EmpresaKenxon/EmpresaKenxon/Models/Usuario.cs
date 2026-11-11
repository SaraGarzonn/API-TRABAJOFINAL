using System.Security.Cryptography;
using System.Text;

namespace EmpresaKenxon.Models
{
    public class Usuario : Persona
    {
        public string NombreUsuario { get; set; }

        // No guardamos la contraseña real, solo su "huella" 
        public string ContrasenaHash { get; set; }

        public Rol Rol { get; set; }

        // Compara la huella de la clave escrita con la huella guardada.
        public bool ValidarContrasena(string clave)
        {
            if (string.IsNullOrEmpty(clave))
            {
                return false;
            }
            return ContrasenaHash == CalcularHash(clave);
        }

        // Convierte un texto en su huella 
        public static string CalcularHash(string clave) //static se usa desde la clase, no desde un objeto
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
            return Convert.ToHexString(bytes);
        }

        public override string ObtenerDescripcion()
        {
            return Nombre + " - usuario " + NombreUsuario + " (" + Rol + ")";
        }
    }
}
