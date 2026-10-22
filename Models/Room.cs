
using System.ComponentModel.DataAnnotations;

namespace HostelHub.Models;

public class Room
{
    public int RoomId { get; set; }

    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string RoomType { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Capacity { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
