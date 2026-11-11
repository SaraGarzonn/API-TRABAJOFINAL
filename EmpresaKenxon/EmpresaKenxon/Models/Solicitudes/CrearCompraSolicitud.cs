namespace EmpresaKenxon.Models.Solicitudes
{
    // Datos que llegan en POST /api/compras
    public class CrearCompraSolicitud
    {
        public int ClienteId { get; set; }
        public string Descripcion { get; set; }
        public decimal Total { get; set; }
        public decimal AbonoInicial { get; set; }
    }
}
