namespace EmpresaKenxon.Models.Solicitudes
{
    // Datos que llegan en POST /api/abonos
    public class CrearAbonoSolicitud
    {
        public int CompraId { get; set; }
        public decimal Monto { get; set; }
    }
}
