using EmpresaKenxon.Models;

namespace EmpresaKenxon.Pruebas
{
    
    public class AbonoPruebas
    {
        
        private Compra CrearCompraConSaldo()
        {
            Compra compra = new Compra { Total = 20000000m };
            compra.AgregarAbonoInicial(new Abono { Monto = 8000000m }, out string mensaje);
            return compra;
        }

       

        [Fact] //este metdodo es una prueba unitaria
        public void Abono_MontoCero_DebeRechazarse()
        {
            Compra compra = CrearCompraConSaldo();

            bool resultado = compra.AgregarAbono(new Abono { Monto = 0m }, out string mensaje);

            Assert.False(resultado);
            Assert.Equal("El monto del abono debe ser mayor a cero.", mensaje);
        }

       
        [Fact]
        public void Abono_MontoNegativo_DebeRechazarse()
        {
            Compra compra = CrearCompraConSaldo();

            bool resultado = compra.AgregarAbono(new Abono { Monto = -500000m }, out string mensaje);

            Assert.False(resultado);
            Assert.Equal("El monto del abono debe ser mayor a cero.", mensaje);
        }

       
        [Fact]
        public void Abono_MayorAlSaldo_DebeRechazarse()
        {
            Compra compra = CrearCompraConSaldo();

            bool resultado = compra.AgregarAbono(new Abono { Monto = 12000001m }, out string mensaje);

            Assert.False(resultado);
            Assert.Contains("no puede ser mayor al saldo", mensaje);
            Assert.Equal(12000000m, compra.CalcularSaldo());
        }

        // Prueba 6
        [Fact]
        public void Abono_Valido_ActualizaSaldo()
        {
            // Preparar
            Compra compra = CrearCompraConSaldo();
            Abono abono = new Abono { Monto = 2000000m };

            // Ejecutar
            bool resultado = compra.AgregarAbono(abono, out string mensaje);

            // Verificar
            Assert.True(resultado);
            Assert.Equal(10000000m, compra.CalcularSaldo());
            Assert.Equal(10000000m, abono.SaldoPosterior);
        }

        // Prueba 9
        [Fact]
        public void Abono_CompraPagada_DebeRechazarse()
        {
            // Preparar: se paga todo el saldo
            Compra compra = CrearCompraConSaldo();
            compra.AgregarAbono(new Abono { Monto = 12000000m }, out string primerMensaje);

            // Ejecutar: se intenta abonar otra vez
            bool resultado = compra.AgregarAbono(new Abono { Monto = 100000m }, out string mensaje);

            // Verificar
            Assert.Equal(EstadoPago.Pagada, compra.EstadoPago);
            Assert.False(resultado);
            Assert.Equal("La compra ya está pagada, no recibe más abonos.", mensaje);
        }
    }
}
