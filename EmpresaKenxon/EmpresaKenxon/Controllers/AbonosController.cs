using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;
using EmpresaKenxon.Models.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpresaKenxon.Controllers
{
    [ApiController]
    [Route("api/abonos")]
    public class AbonosController : ControllerBase
    {
        private readonly IAbonoRepositorio abonoRepositorio;
        private readonly ICompraRepositorio compraRepositorio;
        private readonly IClienteRepositorio clienteRepositorio;
        private readonly IUsuarioRepositorio usuarioRepositorio;

        public AbonosController(IAbonoRepositorio abonoRepositorio, ICompraRepositorio compraRepositorio,
            IClienteRepositorio clienteRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            this.abonoRepositorio = abonoRepositorio;
            this.compraRepositorio = compraRepositorio;
            this.clienteRepositorio = clienteRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
        }

        // POST /api/abonos – Jefe, Secretaria
        // Sigue el diagrama de secuencia (Figura 5) del documento.
        [HttpPost]
        [Authorize(Roles = "Jefe,Secretaria")]
        public IActionResult Crear(CrearAbonoSolicitud solicitud)
        {
            // 1. Consultar la compra
            Compra compra = compraRepositorio.ObtenerPorId(solicitud.CompraId);
            if (compra == null)
            {
                return NotFound(new { mensaje = "No existe una compra con el Id " + solicitud.CompraId + "." });
            }

            // 2. Cargar los abonos existentes para poder calcular el saldo
            compra.Abonos = abonoRepositorio.ObtenerPorCompra(compra.Id);

            Abono abono = new Abono
            {
                Fecha = DateTime.Now,
                Monto = solicitud.Monto,
                CompraId = compra.Id,
                UsuarioId = ObtenerUsuarioId()
            };

            // 3. La compra valida el abono y actualiza saldo y estado
            if (!compra.AgregarAbono(abono, out string mensaje))
            {
                return BadRequest(new { mensaje = mensaje });
            }

            // 4. Guardar el abono y la compra en sus archivos JSON
            abonoRepositorio.Agregar(abono);
            compraRepositorio.Actualizar(compra);

            return StatusCode(201, new
            {
                mensaje = mensaje,
                abonoId = abono.Id,
                compraId = compra.Id,
                monto = abono.Monto,
                saldoPosterior = abono.SaldoPosterior,
                estado = compra.EstadoPago.ToString()
            });
        }

        // GET /api/abonos/{id}/comprobante – Jefe, Secretaria, Empleado
        [HttpGet("{id}/comprobante")]
        [Authorize(Roles = "Jefe,Secretaria,Empleado")]
        public IActionResult Comprobante(int id)
        {
            Abono abono = abonoRepositorio.ObtenerPorId(id);
            if (abono == null)
            {
                return NotFound(new { mensaje = "No existe un abono con el Id " + id + "." });
            }

            Compra compra = compraRepositorio.ObtenerPorId(abono.CompraId);
            Cliente cliente = clienteRepositorio.ObtenerPorId(compra.ClienteId);

            // Buscar el usuario que registró el abono
            Usuario usuario = null;
            foreach (Usuario candidato in usuarioRepositorio.ObtenerTodos())
            {
                if (candidato.Id == abono.UsuarioId)
                {
                    usuario = candidato;
                }
            }

            string registradoPor = "Desconocido";
            if (usuario != null)
            {
                registradoPor = usuario.ObtenerDescripcion();
            }

            return Ok(new
            {
                numeroAbono = abono.Id,
                fecha = abono.Fecha,
                cliente = cliente.ObtenerDescripcion(),
                compra = new { compraId = compra.Id, descripcion = compra.Descripcion, total = compra.Total },
                monto = abono.Monto,
                saldoPosterior = abono.SaldoPosterior,
                registradoPor = registradoPor
            });
        }

        // Lee el Id del usuario que viene dentro del token JWT.
        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirst("Id").Value);
        }
    }
}
