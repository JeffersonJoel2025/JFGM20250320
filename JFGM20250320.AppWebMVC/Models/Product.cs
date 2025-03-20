using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JFGM20250320.AppWebMVC.Models;

public partial class Product
{
    public int ProductId { get; set; }

    
    [Display(Name = "Nombre del usuario")]
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string ProductName { get; set; } = null!;

    [Display(Name = "Nombre del usuario")]
    [Required(ErrorMessage = "La descripcion es obligatorio")]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    [Display(Name = "Bodega")]
    public int? WarehouseId { get; set; }
    [Display(Name = "Marca")]
    public int? BrandId { get; set; }

    [Display(Name = "Marca")]
    [Required(ErrorMessage = "El marca es obligatorio")]
    public virtual Brand? Brand { get; set; }

    [Display(Name = "Bodega")]
    [Required(ErrorMessage = "El bodega es obligatorio")]
    public virtual Warehouse? Warehouse { get; set; }
}
