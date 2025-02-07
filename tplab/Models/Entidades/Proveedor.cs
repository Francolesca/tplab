using System.ComponentModel.DataAnnotations;

namespace tplab.Models.Entidades
{
    public class Proveedor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "EL campo {0} es obligatorio!!!")]
        public string? Nombre { get; set; }
        public List<Marca> Marcas { get; set; }
        public Proveedor()
        {
            Marcas = new List<Marca>();
        }
    }
}
