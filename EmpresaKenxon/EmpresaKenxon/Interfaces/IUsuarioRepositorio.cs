using EmpresaKenxon.Models;

namespace EmpresaKenxon.Interfaces
{
    // que puede hacer con los usuarios
    public interface IUsuarioRepositorio
    {
        List<Usuario> ObtenerTodos();
        Usuario ObtenerPorNombreUsuario(string nombre);
    }
}