using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;

namespace EmpresaKenxon.Repositorios
{
    // Guarda los abonos en una lista en memoria y en el archivo abonos.json.
    public class AbonoRepositorio : IAbonoRepositorio
    {
        private readonly string rutaArchivo;
        private List<Abono> lista = new List<Abono>();

        private readonly JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public AbonoRepositorio(string rutaArchivo)
        {
            this.rutaArchivo = rutaArchivo;
            Cargar();
        }

        public Abono ObtenerPorId(int id)
        {
            foreach (Abono abono in lista)
            {
                if (abono.Id == id)
                {
                    return abono;
                }
            }
            return null;
        }

        // Devuelve una lista NUEVA con los abonos de una compra.
        public List<Abono> ObtenerPorCompra(int compraId)
        {
            List<Abono> resultado = new List<Abono>();
            foreach (Abono abono in lista)
            {
                if (abono.CompraId == compraId)
                {
                    resultado.Add(abono);
                }
            }
            return resultado;
        }

        public void Agregar(Abono abono)
        {
            abono.Id = ObtenerSiguienteId();
            lista.Add(abono);
            Guardar();
        }

        private int ObtenerSiguienteId()
        {
            int mayor = 0;
            foreach (Abono abono in lista)
            {
                if (abono.Id > mayor)
                {
                    mayor = abono.Id;
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
            lista = JsonSerializer.Deserialize<List<Abono>>(json, opciones) ?? new List<Abono>();
        }

        private void Guardar()
        {
            string json = JsonSerializer.Serialize(lista, opciones);
            File.WriteAllText(rutaArchivo, json);
        }
    }
}
