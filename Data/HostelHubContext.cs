
using Microsoft.EntityFrameworkCore;
using HostelHub.Models;

namespace HostelHub.Data;

public class HostelHubContext : DbContext
{
    public HostelHubContext(DbContextOptions<HostelHubContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
}
