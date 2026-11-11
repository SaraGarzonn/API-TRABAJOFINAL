using EmpresaKenxon.Models;
using EmpresaKenxon.Repositorios;

namespace EmpresaKenxon.Pruebas
{
    // prueba de que los repositorios guardan y recuperan los datos desde el archivo JSON.
   
    public class RepositorioPruebas : IDisposable
    {
        private readonly string carpetaTemporal;

        public RepositorioPruebas()
        {
            carpetaTemporal = Path.Combine(Path.GetTempPath(), "KenxonPruebas_" + Guid.NewGuid());
            Directory.CreateDirectory(carpetaTemporal);
        }

        public void Dispose()
        {
            Directory.Delete(carpetaTemporal, true);
        }

        [Fact]
        public void ClienteRepositorio_GuardaYRecuperaDesdeArchivo()
        {
            // Preparar
            string ruta = Path.Combine(carpetaTemporal, "clientes.json");
            ClienteRepositorio repositorio = new ClienteRepositorio(ruta);

            // Ejecutar: se guarda un cliente y se crea un repositorio NUEVO que lee el mismo archivo
            repositorio.Agregar(new Cliente { Cedula = "1036111222", Nombre = "Carlos Gómez", Celular = "3001234567", Direccion = "Rionegro" });
            ClienteRepositorio repositorioNuevo = new ClienteRepositorio(ruta);
            Cliente recuperado = repositorioNuevo.ObtenerPorCedula("1036111222");

            // Verificar
            Assert.True(File.Exists(ruta));
            Assert.NotNull(recuperado);
            Assert.Equal(1, recuperado.Id);
            Assert.Equal("Carlos Gómez", recuperado.Nombre);
        }

        [Fact]
        public void CompraRepositorio_GuardaYRecuperaDesdeArchivo()
        {
            string ruta = Path.Combine(carpetaTemporal, "compras.json");
            CompraRepositorio repositorio = new CompraRepositorio(ruta);

            repositorio.Agregar(new Compra { ClienteId = 1, Descripcion = "Amplificadores", Total = 15000000m, EstadoPago = EstadoPago.Parcial });
            CompraRepositorio repositorioNuevo = new CompraRepositorio(ruta);
            Compra recuperada = repositorioNuevo.ObtenerPorId(1);

            Assert.NotNull(recuperada);
            Assert.Equal(15000000m, recuperada.Total);
            Assert.Equal(EstadoPago.Parcial, recuperada.EstadoPago);
            Assert.Single(repositorioNuevo.ObtenerPorCliente(1));
        }

        [Fact]
        public void AbonoRepositorio_GuardaYRecuperaDesdeArchivo()
        {
            string ruta = Path.Combine(carpetaTemporal, "abonos.json");
            AbonoRepositorio repositorio = new AbonoRepositorio(ruta);

            repositorio.Agregar(new Abono { CompraId = 1, Monto = 8000000m, UsuarioId = 2 });
            repositorio.Agregar(new Abono { CompraId = 1, Monto = 1000000m, UsuarioId = 2 });
            repositorio.Agregar(new Abono { CompraId = 2, Monto = 9000000m, UsuarioId = 1 });
            AbonoRepositorio repositorioNuevo = new AbonoRepositorio(ruta);

            Assert.Equal(2, repositorioNuevo.ObtenerPorCompra(1).Count);
            Assert.Equal(9000000m, repositorioNuevo.ObtenerPorId(3).Monto);
        }

        [Fact]
        public void UsuarioRepositorio_SinArchivo_CreaLosTresUsuarios()
        {
            string ruta = Path.Combine(carpetaTemporal, "usuarios.json");

            UsuarioRepositorio repositorio = new UsuarioRepositorio(ruta);
            UsuarioRepositorio repositorioNuevo = new UsuarioRepositorio(ruta);

            Assert.True(File.Exists(ruta));
            Assert.Equal(3, repositorioNuevo.ObtenerTodos().Count);
            Assert.Equal(Rol.Jefe, repositorioNuevo.ObtenerPorNombreUsuario("jefe").Rol);
            Assert.True(repositorioNuevo.ObtenerPorNombreUsuario("empleado").ValidarContrasena("Empleado123*"));
        }
    }
}
