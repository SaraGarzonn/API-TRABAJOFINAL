using EmpresaKenxon.Models;

namespace EmpresaKenxon.Pruebas
{
    
    public class EstadoPagoPruebas
    {
      
        [Fact]
        public void Compra_ConSaldoPendiente_QuedaParcial()
        {
         
            Compra compra = new Compra { Total = 20000000m };

           
            compra.AgregarAbonoInicial(new Abono { Monto = 8000000m }, out string mensaje);

           
            Assert.Equal(EstadoPago.Parcial, compra.EstadoPago);
        }

        [Fact]
        public void Compra_SaldoEnCero_QuedaPagada()
        {
            
            Compra compra = new Compra { Total = 10000000m };
            compra.AgregarAbonoInicial(new Abono { Monto = 8000000m }, out string mensajeInicial);

            // Ejecutar: se abona exactamente lo que falta
            compra.AgregarAbono(new Abono { Monto = 2000000m }, out string mensaje);

            
            Assert.Equal(0m, compra.CalcularSaldo());
            Assert.Equal(EstadoPago.Pagada, compra.EstadoPago);
        }
    }
}
