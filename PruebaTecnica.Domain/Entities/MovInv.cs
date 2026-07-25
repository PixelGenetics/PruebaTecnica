using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnica.Domain.Entities;

    public class MovInv
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public string Referencia { get; set; } = string.Empty;

        public Product Product { get; set; } = null!;
    }