using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVALORA.Core.Domain.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    [DisplayName("Category Name")]
    public string Name { get; set; } = null!;
}

