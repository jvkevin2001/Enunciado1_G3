using Dapper;
using Microsoft.AspNetCore.Mvc;
using Proyecto_API.Models;
using System.Data;
using System.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class InventarioController : ControllerBase
{
    private readonly string _connectionString;

    public InventarioController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    /// <summary>
    /// Endpoint para obtener la lista completa de productos del inventario.
    /// </summary>
    [HttpGet]
    [Route("Listar")]
    public async Task<IActionResult> ListarProductos()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Llama a tu procedimiento almacenado 'p_GetInventario'
                var productos = await connection.QueryAsync<Inventario>(
                    "p_GetInventario",
                    commandType: CommandType.StoredProcedure
                );
                return Ok(productos);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Endpoint para obtener un producto por su ID.
    /// </summary>
    [HttpGet]
    [Route("Obtener/{idInventario}")]
    public async Task<IActionResult> ObtenerProducto(int idInventario)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Llama a tu procedimiento 'p_GetInventarioById'
                var producto = await connection.QueryFirstOrDefaultAsync<Inventario>(
                    "p_GetInventarioById",
                    new { Id_Inventario = idInventario },
                    commandType: CommandType.StoredProcedure
                );

                if (producto == null)
                {
                    return NotFound("Producto no encontrado.");
                }
                return Ok(producto);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Endpoint para registrar un nuevo producto en el inventario.
    /// </summary>
    [HttpPost]
    [Route("Crear")]
    public async Task<IActionResult> CrearProducto([FromBody] Inventario nuevoProducto)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Llama a tu procedimiento 'p_InsertInventario'
                await connection.ExecuteAsync(
                    "p_InsertInventario",
                    new
                    {
                        nuevoProducto.ProductoNombre,
                        nuevoProducto.Cantidad,
                        nuevoProducto.Descripcion,
                        nuevoProducto.Proveedor,
                        nuevoProducto.PrecioUnitario,
                        nuevoProducto.Id_Usuario
                    },
                    commandType: CommandType.StoredProcedure
                );
                return Ok("Producto creado exitosamente.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Endpoint para actualizar un producto existente.
    /// </summary>
    [HttpPut]
    [Route("Actualizar/{idInventario}")]
    public async Task<IActionResult> ActualizarProducto(int idInventario, [FromBody] Inventario productoActualizado)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Llama a tu procedimiento 'p_UpdateInventario'
                await connection.ExecuteAsync(
                    "p_UpdateInventario",
                    new
                    {
                        Id_Inventario = idInventario,
                        productoActualizado.ProductoNombre,
                        productoActualizado.Cantidad,
                        productoActualizado.Descripcion,
                        productoActualizado.Proveedor,
                        productoActualizado.PrecioUnitario
                    },
                    commandType: CommandType.StoredProcedure
                );
                return Ok("Producto actualizado exitosamente.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Endpoint para eliminar un producto del inventario.
    /// </summary>
    [HttpDelete]
    [Route("Eliminar/{idInventario}")]
    public async Task<IActionResult> EliminarProducto(int idInventario)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Llama a tu procedimiento 'p_DeleteInventario'
                await connection.ExecuteAsync(
                    "p_DeleteInventario",
                    new { Id_Inventario = idInventario },
                    commandType: CommandType.StoredProcedure
                );
                return Ok("Producto eliminado exitosamente.");
            }
        }
        catch (Exception ex)
        {
            // Capturamos el error de SQL en caso de que no se pueda borrar
            if (ex is SqlException sqlEx && sqlEx.Number == 50000)
            {
                return BadRequest(sqlEx.Message);
            }
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }
}
