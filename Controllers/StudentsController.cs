
using HostelHub.Data;
using HostelHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Controllers;

public class StudentsController : Controller
{
    private readonly HostelHubContext _context;

    public StudentsController(HostelHubContext context)
    {
        _context = context;
    }

    // Display all students and their assigned rooms
    public async Task<IActionResult> Index()
    {
        var students = await _context.Students
            .Include(s => s.Room)
            .AsNoTracking()
            .ToListAsync();

        return View(students);
    }

    // Display student registration form
    public async Task<IActionResult> Create()
    {
        await LoadRoomsAsync();
        return View();
    }

    // Save a new student
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        await ValidateRoomAllocationAsync(student);

        if (!ModelState.IsValid)
        {
            await LoadRoomsAsync(student.RoomId);
            return View(student);
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Display edit form
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        await LoadRoomsAsync(student.RoomId);
        return View(student);
    }

    // Update student details
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.StudentId)
            return NotFound();

        await ValidateRoomAllocationAsync(student);

        if (!ModelState.IsValid)
        {
            await LoadRoomsAsync(student.RoomId);
            return View(student);
        }

        var existingStudent = await _context.Students.FindAsync(id);

        if (existingStudent == null)
            return NotFound();

        existingStudent.Name = student.Name;
        existingStudent.Email = student.Email;
        existingStudent.Phone = student.Phone;
        existingStudent.Course = student.Course;
        existingStudent.RoomId = student.RoomId;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Display delete confirmation
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students
            .Include(s => s.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            return NotFound();

        return View(student);
    }

    // Delete student
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Check room capacity before allocating a student
    private async Task ValidateRoomAllocationAsync(Student student)
    {
        if (student.RoomId == null)
            return;

        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomId == student.RoomId);

        if (room == null)
        {
            ModelState.AddModelError(
                nameof(student.RoomId),
                "Please select a valid room.");

            return;
        }

        int occupiedBeds = await _context.Students
            .CountAsync(s => s.RoomId == student.RoomId
                && s.StudentId != student.StudentId);

        if (occupiedBeds >= room.Capacity)
        {
            ModelState.AddModelError(
                nameof(student.RoomId),
                "This room is full. Please select another room.");
        }
    }

    private async Task LoadRoomsAsync(int? selectedRoomId = null)
    {
        var rooms = await _context.Rooms
            .AsNoTracking()
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();

        ViewBag.RoomId = new SelectList(
            rooms,
            nameof(Room.RoomId),
            nameof(Room.RoomNumber),
            selectedRoomId);
    }
}
