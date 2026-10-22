
using System.Diagnostics;
using HostelHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HostelHub.Models;

namespace HostelHub.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HostelHubContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        HostelHubContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalRooms = await _context.Rooms.CountAsync();
        ViewBag.TotalStudents = await _context.Students.CountAsync();

        ViewBag.OccupiedRooms = await _context.Rooms
            .CountAsync(r => r.Students.Any());

        ViewBag.VacantBeds = await _context.Rooms
            .Select(r => r.Capacity - r.Students.Count())
            .SumAsync();

        ViewBag.PendingComplaints = await _context.Complaints
            .CountAsync(c => c.Status != "Resolved");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
