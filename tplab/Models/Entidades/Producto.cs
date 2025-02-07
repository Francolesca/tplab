using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace tplab.Models.Entidades
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio!!!")]
        public string? Nombre { get; set; }
        public int MarcaId { get; set; }
        public Marca? Marca { get; set; }
        public List<Lote> Lotes { get; set; }
        public Producto()
        {
            Lotes = new List<Lote>();
        }
    }
}
