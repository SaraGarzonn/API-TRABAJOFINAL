namespace EmpresaKenxon.Models.Solicitudes
{
    // Datos que llegan en POST /api/clientes
    public class CrearClienteSolicitud
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }
    }
}
