namespace EmpresaKenxon.Models
{
    // molde general
    public abstract class Persona
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }

        public abstract string ObtenerDescripcion();
    }
}
