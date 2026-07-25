using System.ComponentModel.DataAnnotations;

namespace EvaluacionTecnica.Models
{
    public class UpdateProductDto
    {
        //public int Id { get; set; }
        [MaxLength(50)]
        public required string Codigo { get; set; }
        [MaxLength(150)]
        public required string Nombre { get; set; }
        //public int CategoryId { get; set; }
        public decimal Precio { get; set; }
        public bool Estado { get; set; } = true;
    }
}
