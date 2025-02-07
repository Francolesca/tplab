using System.ComponentModel.DataAnnotations;

namespace tplab.Models.Entidades
{
    public class Marca
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio!!!")]
        public string? Nombre { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }
        public List<Producto> Productos { get; set; }
        public Marca()
        {
            Productos = new List<Producto>();
        }
    }
}
