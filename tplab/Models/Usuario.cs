using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace tplab.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
