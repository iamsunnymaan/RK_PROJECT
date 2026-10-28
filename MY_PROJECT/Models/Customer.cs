using System.ComponentModel.DataAnnotations;

namespace MY_PROJECT.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Enter 7-15 digits, optionally starting with +")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Created On")]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [Display(Name = "Updated On")]
    public DateTime? UpdatedOn { get; set; }
}
