namespace EmpresaKenxon.Models.Solicitudes
{
    // Datos que llegan en PUT /api/clientes/{id}
    // No trae cédula: la cédula de un cliente no se puede cambiar.
    public class EditarClienteSolicitud
    {
        public string Nombre { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }
    }
}
