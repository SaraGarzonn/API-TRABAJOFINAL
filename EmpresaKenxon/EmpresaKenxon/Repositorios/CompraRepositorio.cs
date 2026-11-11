using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;

namespace EmpresaKenxon.Repositorios
{

    public class CompraRepositorio : ICompraRepositorio
    {
        private readonly string rutaArchivo;
        private List<Compra> lista = new List<Compra>();

        private readonly JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public CompraRepositorio(string rutaArchivo)
        {
            this.rutaArchivo = rutaArchivo;
            Cargar();
        }

        public List<Compra> ObtenerTodos()
        {
            return lista;
        }

        public Compra ObtenerPorId(int id)
        {
            foreach (Compra compra in lista)
            {
                if (compra.Id == id)
                {
                    return compra;
                }
            }
            return null;
        }

        public List<Compra> ObtenerPorCliente(int clienteId)
        {
            List<Compra> resultado = new List<Compra>();
            foreach (Compra compra in lista)
            {
                if (compra.ClienteId == clienteId)
                {
                    resultado.Add(compra);
                }
            }
            return resultado;
        }

        public void Agregar(Compra compra)
        {
            compra.Id = ObtenerSiguienteId();
            lista.Add(compra);
            Guardar();
        }

        public void Actualizar(Compra compra)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].Id == compra.Id)
                {
                    lista[i] = compra;
                }
            }
            Guardar();
        }

        private int ObtenerSiguienteId()
        {
            int mayor = 0;
            foreach (Compra compra in lista)
            {
                if (compra.Id > mayor)
                {
                    mayor = compra.Id;
                }
            }
            return mayor + 1;
        }

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
            lista = JsonSerializer.Deserialize<List<Compra>>(json, opciones) ?? new List<Compra>();
        }

        private void Guardar()
        {
            string json = JsonSerializer.Serialize(lista, opciones);
            File.WriteAllText(rutaArchivo, json);
        }
    }
}
