namespace ProyectoFinal_G3.Models
{
    public class RespuestaEstandar
    {
        public int Codigo { get; set; }
        public string? Mensaje { get; set; }
        public object? Contenido { get; set; }
    }

    public class RespuestaEstandar<T>
    {
        public int Codigo { get; set; }
        public string? Mensaje { get; set; }
        public T? Contenido { get; set; }
    }
}
