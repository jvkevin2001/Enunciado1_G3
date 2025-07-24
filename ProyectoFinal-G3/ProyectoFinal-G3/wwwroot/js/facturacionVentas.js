function agregarProducto() {
    const fila = `
        <tr>
            <td><input type="text" class="form-control bg-dark text-light border-secondary" placeholder="Producto" /></td>
            <td><input type="number" class="form-control bg-dark text-light border-secondary cantidad" onchange="calcularTotal()" /></td>
            <td><input type="number" class="form-control bg-dark text-light border-secondary precio" onchange="calcularTotal()" /></td>
            <td class="subtotal">₡0.00</td>
            <td><button type="button" class="btn btn-danger btn-sm" onclick="this.closest('tr').remove(); calcularTotal();">Eliminar</button></td>
        </tr>`;
    document.querySelector("#tablaProductos tbody").insertAdjacentHTML('beforeend', fila);
}

function calcularTotal() {
    let total = 0;
    document.querySelectorAll("#tablaProductos tbody tr").forEach(row => {
        const cantidad = parseFloat(row.querySelector(".cantidad")?.value || 0);
        const precio = parseFloat(row.querySelector(".precio")?.value || 0);
        const subtotal = cantidad * precio;
        row.querySelector(".subtotal").textContent = `₡${subtotal.toFixed(2)}`;
        total += subtotal;
    });
    document.getElementById("totalFactura").textContent = total.toFixed(2);
}
