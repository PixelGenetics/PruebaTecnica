using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Domain.Entities;

    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }
        public bool Estado { get; set; } = true;
        public ICollection<Product> Products { get; set; } = new List<Product>();

}

