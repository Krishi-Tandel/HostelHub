
using System.ComponentModel.DataAnnotations;

namespace HostelHub.Models;

public class Complaint
{
    public int ComplaintId { get; set; }

    public int StudentId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Student? Student { get; set; }
}
