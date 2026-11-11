using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;
using EmpresaKenxon.Models.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpresaKenxon.Controllers
{
    [ApiController]
    [Route("api/compras")]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraRepositorio compraRepositorio;
        private readonly IAbonoRepositorio abonoRepositorio;
        private readonly IClienteRepositorio clienteRepositorio;

        public ComprasController(ICompraRepositorio compraRepositorio, IAbonoRepositorio abonoRepositorio, IClienteRepositorio clienteRepositorio)
        {
            this.compraRepositorio = compraRepositorio;
            this.abonoRepositorio = abonoRepositorio;
            this.clienteRepositorio = clienteRepositorio;
        }

        // POST /api/compras – Jefe, Secretaria
        // Crea la compra y su abono inicial (mínimo $8.000.000 y no mayor al total).
        [HttpPost]
        [Authorize(Roles = "Jefe,Secretaria")]
        public IActionResult Crear(CrearCompraSolicitud solicitud)
        {
            Cliente cliente = clienteRepositorio.ObtenerPorId(solicitud.ClienteId);
            if (cliente == null)
            {
                return NotFound(new { mensaje = "No existe un cliente con el Id " + solicitud.ClienteId + "." });
            }

            Compra compra = new Compra
            {
                Fecha = DateTime.Now,
                ClienteId = solicitud.ClienteId,
                Descripcion = solicitud.Descripcion,
                Total = solicitud.Total
            };

            Abono abonoInicial = new Abono
            {
                Fecha = DateTime.Now,
                Monto = solicitud.AbonoInicial,
                UsuarioId = ObtenerUsuarioId()
            };

            // La compra valida la regla del abono inicial.
            if (!compra.AgregarAbonoInicial(abonoInicial, out string mensaje))
            {
                return BadRequest(new { mensaje = mensaje });
            }

            compraRepositorio.Agregar(compra);      // aquí la compra recibe su Id
            abonoInicial.CompraId = compra.Id;
            abonoRepositorio.Agregar(abonoInicial); // aquí el abono recibe su Id

            return StatusCode(201, new
            {
                mensaje = mensaje,
                compraId = compra.Id,
                clienteId = compra.ClienteId,
                descripcion = compra.Descripcion,
                total = compra.Total,
                saldo = compra.CalcularSaldo(),
                estado = compra.EstadoPago.ToString(),
                abonoInicialId = abonoInicial.Id
            });
        }

        // Lee el Id del usuario que viene dentro del token JWT.
        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirst("Id").Value);
        }
    }
}
