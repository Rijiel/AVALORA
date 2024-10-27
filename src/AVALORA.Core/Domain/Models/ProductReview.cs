using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVALORA.Core.Domain.Models;

public class ProductReview
{
    public int Id { get; set; }

	[StringLength(200)]
    public string? Comment { get; set; }

    [Required]
	public DateTime DatePosted { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}

