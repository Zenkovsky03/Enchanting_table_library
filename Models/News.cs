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

    public DateTime PublishDate { get; set; } = DateTime.UtcNow;

    public bool IsPublished { get; set; } = true;
}
