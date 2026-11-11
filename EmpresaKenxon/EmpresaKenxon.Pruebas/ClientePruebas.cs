using EmpresaKenxon.Models;

namespace EmpresaKenxon.Pruebas
{
    
    public class ClientePruebas
    {
        [Fact]
        public void Cliente_DatosCorrectos_DebeRegistrarse()
        {
            
            List<Cliente> clientesRegistrados = new List<Cliente>();
            Cliente cliente = new Cliente { Cedula = "1036111222", Nombre = "Carlos Gómez", Celular = "3001234567", Direccion = "Rionegro" };

            
            bool resultado = cliente.ValidarRegistro(clientesRegistrados, out string mensaje);

            
            Assert.True(resultado);
            Assert.Equal("Cliente registrado correctamente.", mensaje);
        }

        [Fact]
        public void Cliente_CedulaRepetida_DebeRechazarse()
        {

            List<Cliente> clientesRegistrados = new List<Cliente>
            {
                new Cliente { Id = 1, Cedula = "1036111222", Nombre = "Carlos Gómez", Celular = "3001234567", Direccion = "Rionegro" }
            };
            Cliente nuevo = new Cliente { Cedula = "1036111222", Nombre = "Otra Persona", Celular = "3109876543", Direccion = "Marinilla" };

            
            bool resultado = nuevo.ValidarRegistro(clientesRegistrados, out string mensaje);

            
            Assert.False(resultado);
            Assert.Contains("Ya existe un cliente", mensaje);
        }
    }
}
