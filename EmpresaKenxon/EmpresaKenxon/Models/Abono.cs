namespace EmpresaKenxon.Models
{
    // Cada pago que hace el cliente a una compra.
    public class Abono
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public int CompraId { get; set; }

        // Id del usuario que registró el abono (se toma del token JWT).
        public int UsuarioId { get; set; }

        // Saldo que quedó en la compra justo después de este abono.
        public decimal SaldoPosterior { get; set; }
    }
}
