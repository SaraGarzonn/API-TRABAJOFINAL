using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Models;
using EmpresaKenxon.Models.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpresaKenxon.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepositorio clienteRepositorio;
        private readonly ICompraRepositorio compraRepositorio;
        private readonly IAbonoRepositorio abonoRepositorio;

        public ClientesController(IClienteRepositorio clienteRepositorio, ICompraRepositorio compraRepositorio, IAbonoRepositorio abonoRepositorio)
        {
            this.clienteRepositorio = clienteRepositorio;
            this.compraRepositorio = compraRepositorio;
            this.abonoRepositorio = abonoRepositorio;
        }

        // POST /api/clientes – Jefe, Secretaria
        [HttpPost]
        [Authorize(Roles = "Jefe,Secretaria")]
        public IActionResult Crear(CrearClienteSolicitud solicitud)
        {
            Cliente cliente = new Cliente
            {
                Cedula = solicitud.Cedula,
                Nombre = solicitud.Nombre,
                Celular = solicitud.Celular,
                Direccion = solicitud.Direccion
            };

            // La regla de la cédula repetida la valida el modelo Cliente.
            if (!cliente.ValidarRegistro(clienteRepositorio.ObtenerTodos(), out string mensaje))
            {
                return BadRequest(new { mensaje = mensaje });
            }

            clienteRepositorio.Agregar(cliente);
            return StatusCode(201, cliente); // 201 = creado
        }

        // GET /api/clientes – Jefe, Secretaria, Empleado
        [HttpGet]
        [Authorize(Roles = "Jefe,Secretaria,Empleado")]
        public IActionResult ObtenerTodos()
        {
            return Ok(clienteRepositorio.ObtenerTodos());
        }

        // GET /api/clientes/{id} – Jefe, Secretaria, Empleado
        [HttpGet("{id}")]
        [Authorize(Roles = "Jefe,Secretaria,Empleado")]
        public IActionResult ObtenerPorId(int id)
        {
            Cliente cliente = clienteRepositorio.ObtenerPorId(id);
            if (cliente == null)
            {
                return NotFound(new { mensaje = "No existe un cliente con el Id " + id + "." });
            }
            return Ok(cliente);
        }

        // PUT /api/clientes/{id} – Jefe, Secretaria
        // Solo cambia nombre, celular y dirección. La cédula se conserva.
        [HttpPut("{id}")]
        [Authorize(Roles = "Jefe,Secretaria")]
        public IActionResult Editar(int id, EditarClienteSolicitud solicitud)
        {
            Cliente existente = clienteRepositorio.ObtenerPorId(id);
            if (existente == null)
            {
                return NotFound(new { mensaje = "No existe un cliente con el Id " + id + "." });
            }

            Cliente clienteEditado = new Cliente
            {
                Id = existente.Id,
                Cedula = existente.Cedula,
                Nombre = solicitud.Nombre,
                Celular = solicitud.Celular,
                Direccion = solicitud.Direccion
            };

            if (!clienteEditado.ValidarDatos(out string mensaje))
            {
                return BadRequest(new { mensaje = mensaje });
            }

            clienteRepositorio.Actualizar(clienteEditado);
            return Ok(clienteEditado);
        }

        // GET /api/clientes/{id}/estado-cuenta – Jefe, Secretaria, Empleado
        [HttpGet("{id}/estado-cuenta")]
        [Authorize(Roles = "Jefe,Secretaria,Empleado")]
        public IActionResult EstadoCuenta(int id)
        {
            Cliente cliente = clienteRepositorio.ObtenerPorId(id);
            if (cliente == null)
            {
                return NotFound(new { mensaje = "No existe un cliente con el Id " + id + "." });
            }

            List<object> compras = new List<object>();
            List<Abono> abonos = new List<Abono>();
            decimal saldoTotal = 0;

            foreach (Compra compra in compraRepositorio.ObtenerPorCliente(id))
            {
                // Primero cargamos los abonos de la compra para poder calcular su saldo.
                compra.Abonos = abonoRepositorio.ObtenerPorCompra(compra.Id);

                // "new { ... }" es un objeto anónimo: una respuesta armada en el momento sin crear una clase.
                compras.Add(new
                {
                    compraId = compra.Id,
                    fecha = compra.Fecha,
                    descripcion = compra.Descripcion,
                    total = compra.Total,
                    abonado = compra.CalcularTotalAbonado(),
                    saldo = compra.CalcularSaldo(),
                    estado = compra.EstadoPago.ToString()
                });

                abonos.AddRange(compra.Abonos);
                saldoTotal = saldoTotal + compra.CalcularSaldo();
            }

            return Ok(new
            {
                cliente = cliente,
                compras = compras,
                abonos = abonos,
                saldoTotal = saldoTotal
            });
        }
    }
}
