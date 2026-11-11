using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;

namespace EmpresaKenxon.Repositorios
{
    // Guarda los clientes en una lista e y en clientes.json.
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly string rutaArchivo;//se asigna una vez y no cambia
        private List<Cliente> lista = new List<Cliente>();

        private readonly JsonSerializerOptions opciones = new JsonSerializerOptions //le dice como escribe el archivo
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public ClienteRepositorio(string rutaArchivo)
        {
            this.rutaArchivo = rutaArchivo;
            Cargar();
        }

        public List<Cliente> ObtenerTodos()
        {
            return lista;
        }

        public Cliente ObtenerPorId(int id)
        {
            foreach (Cliente cliente in lista)
            {
                if (cliente.Id == id)
                {
                    return cliente;
                }
            }
            return null;
        }

        public Cliente ObtenerPorCedula(string cedula)
        {
            foreach (Cliente cliente in lista)
            {
                if (cliente.Cedula == cedula)
                {
                    return cliente;
                }
            }
            return null;
        }

        //  Id siguente, lo agrega y guarda el archivo.
        public void Agregar(Cliente cliente)
        {
            cliente.Id = ObtenerSiguienteId();
            lista.Add(cliente);
            Guardar();
        }

        // Reemplaza el cliente que tenga el mismo Id y guarda el archivo.
        public void Actualizar(Cliente cliente)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].Id == cliente.Id)
                {
                    lista[i] = cliente;
                }
            }
            Guardar();
        }

        private int ObtenerSiguienteId()
        {
            int mayor = 0;
            foreach (Cliente cliente in lista)
            {
                if (cliente.Id > mayor)
                {
                    mayor = cliente.Id;
                }
            }
            return mayor + 1;
        }

        // Lee el archivo. Si no existe, lo crea vacío ("[]").
        private void Cargar()
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            if (!File.Exists(rutaArchivo))
            {
                File.WriteAllText(rutaArchivo, "[]");
            }

            string json = File.ReadAllText(rutaArchivo);
            lista = JsonSerializer.Deserialize<List<Cliente>>(json, opciones) ?? new List<Cliente>();
        }

        private void Guardar()
        {
            string json = JsonSerializer.Serialize(lista, opciones);
            File.WriteAllText(rutaArchivo, json);
        }
    }
}
