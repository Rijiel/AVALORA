﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Domain.Models;

public class CartItem
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = null!;

    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser? ApplicationUser { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "You can only order maximum 20 items at a time")]
    public int Count { get; set; }

    [Required(ErrorMessage = "Please specify your preferred color")]
    public Color? Color { get; set; }

	[NotMapped]
	public double TotalPrice { get; set; }
}

