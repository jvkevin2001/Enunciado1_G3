var productos = [];

function inicializarProductos(productosServer) {
    productos = productosServer;
}

function agregarFila() {
    var tbody = document.querySelector("#productosTabla tbody");
    var filaIndex = tbody.children.length;

    var fila = document.createElement("tr");
    fila.innerHTML = `
        <td>
            <select name="detalles[${filaIndex}].Id_Inventario" class="form-select bg-dark text-light border-secondary productoSelect">
                <option value="">Seleccione</option>
                ${productos.map(p => `<option value="${p.Id_Inventario}" data-precio="${p.PrecioUnitario}">${p.ProductoNombre}</option>`).join('')}
            </select>
        </td>
        <td><input type="number" name="detalles[${filaIndex}].Cantidad" class="form-control bg-dark text-light border-secondary cantidadInput" value="1" min="1"></td>
        <td><input type="text" class="form-control bg-dark text-light border-secondary precioInput" readonly></td>
        <td><input type="text" class="form-control bg-dark text-light border-secondary totalInput" readonly></td>
        <td><button type="button" class="btn btn-sm btn-outline-danger eliminarBtn">Eliminar</button></td>
    `;
    tbody.appendChild(fila);

    fila.querySelectorAll(".productoSelect, .cantidadInput").forEach(el => el.addEventListener("change", actualizarTotales));
    fila.querySelector(".eliminarBtn").addEventListener("click", function () {
        eliminarFila(fila);
    });

    actualizarTotales();
}

function eliminarFila(fila) {
    fila.remove();
    document.querySelectorAll("#productosTabla tbody tr").forEach((fila, i) => {
        fila.querySelector(".productoSelect").setAttribute("name", `detalles[${i}].Id_Inventario`);
        fila.querySelector(".cantidadInput").setAttribute("name", `detalles[${i}].Cantidad`);
    });
    actualizarTotales();
}

function actualizarTotales() {
    var total = 0;
    document.querySelectorAll("#productosTabla tbody tr").forEach(fila => {
        var productoSelect = fila.querySelector(".productoSelect");
        var cantidad = parseFloat(fila.querySelector(".cantidadInput").value) || 0;
        var precio = parseFloat(productoSelect.selectedOptions[0]?.dataset.precio || 0);

        fila.querySelector(".precioInput").value = precio.toFixed(2);
        fila.querySelector(".totalInput").value = (precio * cantidad).toFixed(2);
        total += precio * cantidad;
    });
    document.getElementById("totalVenta").value = total.toFixed(2);
}

document.addEventListener("DOMContentLoaded", function () {
    var btnAgregar = document.getElementById("btnAgregarProducto");
    if (btnAgregar) {
        btnAgregar.addEventListener("click", agregarFila);
    }
});
