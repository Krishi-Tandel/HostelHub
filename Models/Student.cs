
using System.ComponentModel.DataAnnotations;

namespace HostelHub.Models;

public class Student
{
    public int StudentId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Course { get; set; } = string.Empty;

    public int? RoomId { get; set; }

    public Room? Room { get; set; }

    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
