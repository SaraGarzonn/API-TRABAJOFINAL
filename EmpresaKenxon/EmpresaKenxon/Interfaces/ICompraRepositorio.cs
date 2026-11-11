using EmpresaKenxon.Models;

namespace EmpresaKenxon.Interfaces
{
    public interface ICompraRepositorio
    {
        List<Compra> ObtenerTodos();
        Compra ObtenerPorId(int id);
        List<Compra> ObtenerPorCliente(int clienteId);
        void Agregar(Compra compra);
        void Actualizar(Compra compra);
    }
}
