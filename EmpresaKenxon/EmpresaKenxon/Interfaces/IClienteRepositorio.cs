using EmpresaKenxon.Models;

namespace EmpresaKenxon.Interfaces
{
    public interface IClienteRepositorio
    {
        List<Cliente> ObtenerTodos();
        Cliente ObtenerPorId(int id);
        Cliente ObtenerPorCedula(string cedula);
        void Agregar(Cliente cliente);
        void Actualizar(Cliente cliente);
    }
}

