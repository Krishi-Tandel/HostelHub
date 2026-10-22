
using HostelHub.Data;
using HostelHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Controllers;

public class RoomsController : Controller
{
    private readonly HostelHubContext _context;

    public RoomsController(HostelHubContext context)
    {
        _context = context;
    }

    // Display all rooms
    public async Task<IActionResult> Index()
    {
        var rooms = await _context.Rooms
            .AsNoTracking()
            .ToListAsync();

        return View(rooms);
    }

    // Display room details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
            return NotFound();

        return View(room);
    }

    // Display the create form
    public IActionResult Create()
    {
        return View();
    }

    // Save the new room
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Room room)
    {
        if (!ModelState.IsValid)
            return View(room);

        bool roomExists = await _context.Rooms
            .AnyAsync(r => r.RoomNumber == room.RoomNumber);

        if (roomExists)
        {
            ModelState.AddModelError(
                nameof(room.RoomNumber),
                "This room number already exists.");

            return View(room);
        }

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Display the edit form
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            return NotFound();

        return View(room);
    }

    // Save room changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Room room)
    {
        if (id != room.RoomId)
            return NotFound();

        if (!ModelState.IsValid)
            return View(room);

        bool roomExists = await _context.Rooms
            .AnyAsync(r => r.RoomNumber == room.RoomNumber
                        && r.RoomId != room.RoomId);

        if (roomExists)
        {
            ModelState.AddModelError(
                nameof(room.RoomNumber),
                "This room number already exists.");

            return View(room);
        }

        var existingRoom = await _context.Rooms.FindAsync(id);

        if (existingRoom == null)
            return NotFound();

        existingRoom.RoomNumber = room.RoomNumber;
        existingRoom.RoomType = room.RoomType;
        existingRoom.Capacity = room.Capacity;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Display delete confirmation
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
            return NotFound();

        return View(room);
    }

    // Delete a room
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.Students)
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
            return NotFound();

        if (room.Students.Any())
        {
            TempData["Error"] =
                "This room cannot be deleted because students are assigned to it.";

            return RedirectToAction(nameof(Index));
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
