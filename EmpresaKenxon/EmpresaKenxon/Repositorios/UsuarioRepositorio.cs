using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;

namespace EmpresaKenxon.Repositorios
{
  
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string rutaArchivo;
        private List<Usuario> lista = new List<Usuario>();

        private readonly JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public UsuarioRepositorio(string rutaArchivo)
        {
            this.rutaArchivo = rutaArchivo;
            Cargar();
        }

        public List<Usuario> ObtenerTodos()
        {
            return lista;
        }

        public Usuario ObtenerPorNombreUsuario(string nombre)
        {
            foreach (Usuario usuario in lista)
            {
                if (usuario.NombreUsuario == nombre)
                {
                    return usuario;
                }
            }
            return null;
        }

        
        private void Cargar()
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(carpeta))
            {
                Directory.CreateDirectory(carpeta);// Crea la carpeta si no existe
            }

            if (!File.Exists(rutaArchivo))
            {
                lista = CrearUsuariosIniciales();
                Guardar();
                return;
            }

            string json = File.ReadAllText(rutaArchivo);
            lista = JsonSerializer.Deserialize<List<Usuario>>(json, opciones) ?? new List<Usuario>();// Si falla, inicializa una lista vacía
        }

        // Reescribe el archivo completo con el contenido de la lista.
        private void Guardar()
        {
            string json = JsonSerializer.Serialize(lista, opciones);// Convierte la lista a JSON con formato legible
            File.WriteAllText(rutaArchivo, json);
        }

        private List<Usuario> CrearUsuariosIniciales()
        {
            List<Usuario> usuarios = new List<Usuario>();

            usuarios.Add(new Usuario
            {
                Id = 1,
                Cedula = "1000000001",
                Nombre = "Jefe Kenxon",
                Celular = "3000000001",
                Direccion = "Rionegro, Antioquia",
                NombreUsuario = "jefe",
                ContrasenaHash = Usuario.CalcularHash("Jefe123*"),
                Rol = Rol.Jefe
            });

            usuarios.Add(new Usuario
            {
                Id = 2,
                Cedula = "1000000002",
                Nombre = "Secretaria Kenxon",
                Celular = "3000000002",
                Direccion = "Rionegro, Antioquia",
                NombreUsuario = "secretaria",
                ContrasenaHash = Usuario.CalcularHash("Secretaria123*"),
                Rol = Rol.Secretaria
            });

            usuarios.Add(new Usuario
            {
                Id = 3,
                Cedula = "1000000003",
                Nombre = "Empleado Kenxon",
                Celular = "3000000003",
                Direccion = "Rionegro, Antioquia",
                NombreUsuario = "empleado",
                ContrasenaHash = Usuario.CalcularHash("Empleado123*"),
                Rol = Rol.Empleado
            });

            return usuarios;
        }
    }
}
