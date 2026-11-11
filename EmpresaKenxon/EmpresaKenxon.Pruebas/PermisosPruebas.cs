using System.Reflection;
using EmpresaKenxon.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace EmpresaKenxon.Pruebas
{
    //permisos por rol, reflexion para obtener los roles de los metodos

    public class PermisosPruebas
    {
        private const string SoloRegistran = "Jefe,Secretaria";
        private const string TodosConsultan = "Jefe,Secretaria,Empleado";

        // Devuelve el texto de Roles del [Authorize] de un método, o null si no tiene.
        private string ObtenerRoles(Type controlador, string nombreMetodo)
        {
            MethodInfo metodo = controlador.GetMethod(nombreMetodo);
            AuthorizeAttribute atributo = metodo.GetCustomAttribute<AuthorizeAttribute>();
            if (atributo == null)
            {
                return null;
            }
            return atributo.Roles;
        }

        [Fact]
        public void Registrar_SoloJefeYSecretaria_EmpleadoNoPuede()
        {
            Assert.Equal(SoloRegistran, ObtenerRoles(typeof(ClientesController), "Crear"));
            Assert.Equal(SoloRegistran, ObtenerRoles(typeof(ClientesController), "Editar"));
            Assert.Equal(SoloRegistran, ObtenerRoles(typeof(ComprasController), "Crear"));
            Assert.Equal(SoloRegistran, ObtenerRoles(typeof(AbonosController), "Crear"));

            Assert.DoesNotContain("Empleado", ObtenerRoles(typeof(AbonosController), "Crear"));
        }

        [Fact]
        public void Consultar_LosTresRolesPueden()
        {
            Assert.Equal(TodosConsultan, ObtenerRoles(typeof(ClientesController), "ObtenerTodos"));
            Assert.Equal(TodosConsultan, ObtenerRoles(typeof(ClientesController), "ObtenerPorId"));
            Assert.Equal(TodosConsultan, ObtenerRoles(typeof(ClientesController), "EstadoCuenta"));
            Assert.Equal(TodosConsultan, ObtenerRoles(typeof(AbonosController), "Comprobante"));
            Assert.Equal(TodosConsultan, ObtenerRoles(typeof(ReportesController), "Cartera"));
        }

        [Fact]
        public void Login_EsPublico()//aqui verifica que el login no tenga el atributo Authorize, es publico
        {
            Assert.Null(ObtenerRoles(typeof(AuthController), "Login"));
        }
    }
}
