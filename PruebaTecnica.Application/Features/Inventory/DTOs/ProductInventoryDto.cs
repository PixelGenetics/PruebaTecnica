using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Features.Inventory.DTOs
{
    public class ProductInventoryDto
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public decimal Precio { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int TotalEntradas { get; set; }

        public int TotalSalidas { get; set; }

        public int StockActual { get; set; }

        public List<MovementDto> Movimientos { get; set; } = [];
    }
}
