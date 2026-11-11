using System.Text.Json.Serialization;

namespace EmpresaKenxon.Models
{
    // Compra de un cliente. Aquí viven las reglas de los abonos y del saldo.
    public class Compra
    {
        public const decimal AbonoInicialMinimo = 8000000m;

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public string Descripcion { get; set; }
        public decimal Total { get; set; }
        public EstadoPago EstadoPago { get; set; }

        // [JsonIgnore]: esta lista NO se escribe en compras.json.
        // Los abonos se guardan en abonos.json y el controlador los carga aquí antes de usar la compra.
        [JsonIgnore]
        public List<Abono> Abonos { get; set; } = new List<Abono>();

        // Suma de todos los abonos de la compra.
        public decimal CalcularTotalAbonado()
        {
            decimal suma = 0;
            foreach (Abono abono in Abonos)
            {
                suma = suma + abono.Monto;
            }
            return suma;
        }

        public decimal CalcularSaldo()
        {
            return Total - CalcularTotalAbonado();
        }

        public void ActualizarEstado()
        {
            if (CalcularSaldo() == 0)
            {
                EstadoPago = EstadoPago.Pagada;
            }
            else
            {
                EstadoPago = EstadoPago.Parcial;
            }
        }

        // Primer abono que se registra junto con la compra.
        // mínimo $8.000.000 y no mayor al total.
        public bool AgregarAbonoInicial(Abono abono, out string mensaje)
        {
            if (abono.Monto < AbonoInicialMinimo)
            {
                mensaje = "El abono inicial debe ser de mínimo $8.000.000.";
                return false;
            }

            if (abono.Monto > Total)
            {
                mensaje = "El abono inicial no puede ser mayor al total de la compra.";
                return false;
            }

            Abonos.Add(abono);
            abono.SaldoPosterior = CalcularSaldo();
            ActualizarEstado();
            mensaje = "Compra registrada correctamente.";
            return true;
        }

         
        
        public bool AgregarAbono(Abono abono, out string mensaje)
        {
            if (EstadoPago == EstadoPago.Pagada)
            {
                mensaje = "La compra ya está pagada, no recibe más abonos.";
                return false;
            }

            if (abono.Monto <= 0)
            {
                mensaje = "El monto del abono debe ser mayor a cero.";
                return false;
            }

            if (abono.Monto > CalcularSaldo())
            {
                mensaje = "El abono no puede ser mayor al saldo pendiente de " + CalcularSaldo() + ".";
                return false;
            }

            Abonos.Add(abono);
            abono.SaldoPosterior = CalcularSaldo();
            ActualizarEstado();
            mensaje = "Abono registrado correctamente.";
            return true;
        }
    }
}
