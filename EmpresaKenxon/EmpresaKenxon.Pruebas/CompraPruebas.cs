using EmpresaKenxon.Models;

namespace EmpresaKenxon.Pruebas
{
    
    public class CompraPruebas
    {
        
        [Fact]
        public void Compra_ConAbonoInicial_CalculaSaldoCorrecto()
        {
            
            Compra compra = new Compra { Total = 20000000m };
            Abono abonoInicial = new Abono { Monto = 8000000m };

           
            bool resultado = compra.AgregarAbonoInicial(abonoInicial, out string mensaje);

            
            Assert.True(resultado);
            Assert.Equal(12000000m, compra.CalcularSaldo());
            Assert.Equal(12000000m, abonoInicial.SaldoPosterior);
        }

        // Prueba 3
        [Fact]
        public void AbonoInicial_MenorAlMinimo_DebeRechazarse()
        {
            
            Compra compra = new Compra { Total = 20000000m };
            Abono abonoInicial = new Abono { Monto = 7999999m };

            
            bool resultado = compra.AgregarAbonoInicial(abonoInicial, out string mensaje);

            
            Assert.False(resultado);
            Assert.Equal("El abono inicial debe ser de mínimo $8.000.000.", mensaje);
            Assert.Empty(compra.Abonos);
        }

        
        [Fact]
        public void AbonoInicial_MayorAlTotal_DebeRechazarse()
        {
           
            Compra compra = new Compra { Total = 10000000m };
            Abono abonoInicial = new Abono { Monto = 12000000m };

          
            bool resultado = compra.AgregarAbonoInicial(abonoInicial, out string mensaje);

            
            Assert.False(resultado);
            Assert.Equal("El abono inicial no puede ser mayor al total de la compra.", mensaje);
            Assert.Empty(compra.Abonos);
        }
    }
}
