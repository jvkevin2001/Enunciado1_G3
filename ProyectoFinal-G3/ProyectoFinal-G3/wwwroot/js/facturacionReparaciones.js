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

function agregarServicio() {
    const fila = `
        <tr>
            <td><input type="text" class="form-control bg-dark text-light border-secondary" placeholder="Servicio" /></td>
            <td><input type="number" class="form-control bg-dark text-light border-secondary servicioPrecio" onchange="calcularTotal()" /></td>
            <td><button type="button" class="btn btn-danger btn-sm" onclick="this.closest('tr').remove(); calcularTotal();">Eliminar</button></td>
        </tr>`;
    document.querySelector("#tablaServicios tbody").insertAdjacentHTML('beforeend', fila);
}

function calcularTotal() {
    let total = 0;

    document.querySelectorAll("#tablaProductos tbody tr").forEach(row => {
        const c = parseFloat(row.querySelector(".cantidad")?.value || 0);
        const p = parseFloat(row.querySelector(".precio")?.value || 0);
        const s = c * p;
        row.querySelector(".subtotal").textContent = `₡${s.toFixed(2)}`;
        total += s;
    });

    document.querySelectorAll(".servicioPrecio").forEach(input => {
        total += parseFloat(input.value || 0);
    });

    document.getElementById("totalReparacion").textContent = total.toFixed(2);
}
