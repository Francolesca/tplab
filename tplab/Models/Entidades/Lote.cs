namespace tplab.Models.Entidades
{
    public class Lote
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public DateOnly FechaVencimiento { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}
