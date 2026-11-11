using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpresaKenxon.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly ICompraRepositorio compraRepositorio;
        private readonly IAbonoRepositorio abonoRepositorio;
        private readonly IClienteRepositorio clienteRepositorio;

        public ReportesController(ICompraRepositorio compraRepositorio, IAbonoRepositorio abonoRepositorio, IClienteRepositorio clienteRepositorio)
        {
            this.compraRepositorio = compraRepositorio;
            this.abonoRepositorio = abonoRepositorio;
            this.clienteRepositorio = clienteRepositorio;
        }

        // GET /api/reportes/cartera – Jefe, Secretaria, Empleado
        // Compras en estado Parcial, con su cliente y saldo, y el total pendiente general.
        [HttpGet("cartera")]
        [Authorize(Roles = "Jefe,Secretaria,Empleado")]
        public IActionResult Cartera()
        {
            List<object> comprasPendientes = new List<object>();
            decimal totalPendiente = 0;

            foreach (Compra compra in compraRepositorio.ObtenerTodos())
            {
                if (compra.EstadoPago == EstadoPago.Parcial)
                {
                    compra.Abonos = abonoRepositorio.ObtenerPorCompra(compra.Id);
                    decimal saldo = compra.CalcularSaldo();
                    Cliente cliente = clienteRepositorio.ObtenerPorId(compra.ClienteId);

                    comprasPendientes.Add(new
                    {
                        clienteId = cliente.Id,
                        cedula = cliente.Cedula,
                        nombre = cliente.Nombre,
                        compraId = compra.Id,
                        descripcion = compra.Descripcion,
                        total = compra.Total,
                        saldo = saldo
                    });

                    totalPendiente = totalPendiente + saldo;
                }
            }

            return Ok(new { comprasPendientes = comprasPendientes, totalPendiente = totalPendiente });
        }
    }
}
