using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

public class News
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }
    [ValidateNever]
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;

    public bool IsPublished { get; set; } = true;
}
