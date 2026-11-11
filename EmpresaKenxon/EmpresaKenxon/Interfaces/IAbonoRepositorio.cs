using EmpresaKenxon.Models;

namespace EmpresaKenxon.Interfaces
{
    public interface IAbonoRepositorio
    {
        Abono ObtenerPorId(int id);
        List<Abono> ObtenerPorCompra(int compraId);
        void Agregar(Abono abono);
    }
}
