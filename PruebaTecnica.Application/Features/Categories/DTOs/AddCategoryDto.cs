using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Features.Categories.DTOs;
    public class AddCategoryDto
    {
        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }
        public bool Estado { get; set; }
    }
