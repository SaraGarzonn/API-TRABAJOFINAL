namespace EmpresaKenxon.Models
{
    // Cliente mayorista. Hereda todos sus datos de Persona.
    public class Cliente : Persona
    {
        public override string ObtenerDescripcion()
        {
            return "Cliente mayorista " + Nombre + " - cédula " + Cedula;
        }

        // Revisa que no falte ningún dato obligatorio.
        public bool ValidarDatos(out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(Cedula) || string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Celular) || string.IsNullOrWhiteSpace(Direccion))
            {
                mensaje = "La cédula, el nombre, el celular y la dirección son obligatorios.";
                return false;
            }

            mensaje = "Datos válidos.";
            return true;
        }

        // Revisa los datos y que la cédula no esté repetida entre los clientes ya registrados.
        public bool ValidarRegistro(List<Cliente> clientesRegistrados, out string mensaje)
        {
            if (!ValidarDatos(out mensaje))
            {
                return false;
            }

            foreach (Cliente cliente in clientesRegistrados)
            {
                if (cliente.Cedula == Cedula)
                {
                    mensaje = "Ya existe un cliente con la cédula " + Cedula + ".";
                    return false;
                }
            }

            mensaje = "Cliente registrado correctamente.";
            return true;
        }
    }
}
