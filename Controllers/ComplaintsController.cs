
using HostelHub.Data;
using HostelHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HostelHub.Controllers;

public class ComplaintsController : Controller
{
    private readonly HostelHubContext _context;

    public ComplaintsController(HostelHubContext context)
    {
        _context = context;
    }

    // Display all complaints
    public async Task<IActionResult> Index()
    {
        var complaints = await _context.Complaints
            .Include(c => c.Student)
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(complaints);
    }

    // Display complaint form
    public async Task<IActionResult> Create()
    {
        await LoadStudentsAsync();
        return View();
    }

    // Save complaint
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Complaint complaint)
    {
        await LoadStudentsAsync(complaint.StudentId);

        bool studentExists = await _context.Students
            .AnyAsync(s => s.StudentId == complaint.StudentId);

        if (!studentExists)
        {
            ModelState.AddModelError(
                nameof(complaint.StudentId),
                "Please select a valid student.");
        }

        if (!ModelState.IsValid)
            return View(complaint);

        complaint.Status = "Pending";
        complaint.CreatedAt = DateTime.Now;

        _context.Complaints.Add(complaint);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Update complaint status
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id, string status)
    {
        string[] allowedStatuses =
            { "Pending", "In Progress", "Resolved" };

        if (!allowedStatuses.Contains(status))
            return BadRequest("Invalid complaint status.");

        var complaint = await _context.Complaints.FindAsync(id);

        if (complaint == null)
            return NotFound();

        complaint.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Display delete confirmation
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var complaint = await _context.Complaints
            .Include(c => c.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ComplaintId == id);

        if (complaint == null)
            return NotFound();

        return View(complaint);
    }

    // Delete complaint
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var complaint = await _context.Complaints.FindAsync(id);

        if (complaint == null)
            return NotFound();

        _context.Complaints.Remove(complaint);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadStudentsAsync(int? selectedStudentId = null)
    {
        var students = await _context.Students
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.StudentId = new SelectList(
            students,
            nameof(Student.StudentId),
            nameof(Student.Name),
            selectedStudentId);
    }
}
