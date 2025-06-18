using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/workshift")]
public class WorkshiftController(ILogger<WorkshiftController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<WorkshiftController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var workshifts = _context.Workshifts
            .Include(ws => ws.Therapist)
            .Include(ws => ws.Practice)
            .Select(ws => ws.ToDTO())
            .ToList();
        if (workshifts == null || workshifts.Count == 0)
        {
            _logger.LogInformation("No workshifts found");
            return NotFound("No workshifts found");
        }
        _logger.LogInformation("Fetched all workshifts: {WorkshiftsJson}", JsonSerializer.Serialize(workshifts));
        return Ok(workshifts);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var workshift = _context.Workshifts
            .Include(ws => ws.Therapist)
            .Include(ws => ws.Practice)
            .FirstOrDefault(ws => ws.Id == id);
        if (workshift == null)
        {
            _logger.LogWarning("Workshift with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched workshift: {WorkshiftJson}", JsonSerializer.Serialize(workshift));
        return Ok(workshift.ToDTO());
    }

    [HttpGet("{id}/therapist")]
    public IActionResult GetTherapist(string id)
    {
        var workshift = _context.Workshifts.Include(ws => ws.Therapist).FirstOrDefault(ws => ws.Id == id);
        if (workshift == null)
        {
            _logger.LogWarning("Workshift with ID {Id} not found", id);
            return NotFound();
        }
        var therapist = workshift.Therapist;
        _logger.LogInformation("Fetched therapist for workshift {Id}: {TherapistJson}", id, JsonSerializer.Serialize(therapist));
        return Ok(therapist?.ToDTO());
    }

    [HttpGet("{id}/practice")]
    public IActionResult GetPractice(string id)
    {
        var workshift = _context.Workshifts.Include(ws => ws.Practice).FirstOrDefault(ws => ws.Id == id);
        if (workshift == null)
        {
            _logger.LogWarning("Workshift with ID {Id} not found", id);
            return NotFound();
        }
        var practice = workshift.Practice;
        _logger.LogInformation("Fetched practice for workshift {Id}: {PracticeJson}", id, JsonSerializer.Serialize(practice));
        return Ok(practice?.ToDTO());
    }

    [HttpGet]
    public IActionResult GetFromTimespan([FromQuery] string? therapistId, [FromQuery] string? practiceId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var query = _context.Workshifts.AsQueryable();
        if (!string.IsNullOrEmpty(therapistId))
        {
            query = query.Where(ws => ws.Therapist.Id == therapistId);
        }
        if (!string.IsNullOrEmpty(practiceId))
        {
            query = query.Where(ws => ws.Practice.Id == practiceId);
        }
        if (start.HasValue)
        {
            query = query.Where(ws => ws.StartTime >= start.Value);
        }
        if (end.HasValue)
        {
            query = query.Where(ws => ws.EndTime <= end.Value);
        }
        var workshifts = query
            .Include(ws => ws.Therapist)
            .Include(ws => ws.Practice)
            .ToList()
            .Select(ws => ws.ToDTO())
            .ToList();
        _logger.LogInformation("Fetched workshifts with filters therapistId={TherapistId}, practiceId={PracticeId}, start={Start}, end={End}: {WorkshiftsJson}", therapistId, practiceId, start, end, JsonSerializer.Serialize(workshifts));
        return Ok(workshifts);
    }

    [HttpPost]
    public IActionResult Create([FromBody] WorkshiftSummaryDTO workshift)
    {
        if (workshift == null)
        {
            _logger.LogError("Received null workshift data");
            return BadRequest("Workshift data cannot be null");
        }
        Workshift newWorkshift = ToWorkshift(workshift);
        _context.Workshifts.Add(newWorkshift);
        _context.SaveChanges();
        _logger.LogInformation("Created new workshift: {WorkshiftJson}", JsonSerializer.Serialize(workshift));
        return CreatedAtAction(nameof(Get), new { id = workshift.Id }, newWorkshift.ToDTO());
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Workshift workshift)
    {
        if (workshift == null)
        {
            _logger.LogError("Received invalid workshift data for update");
            _logger.LogInformation("Received workshift data: {WorkshiftJson}", JsonSerializer.Serialize(workshift));
            return BadRequest("Invalid workshift data");
        }
        var existing = _context.Workshifts
            .Include(ws => ws.Therapist)
            .Include(ws => ws.Practice)
            .FirstOrDefault(ws => ws.Id == id);
        if (existing == null)
        {
            _logger.LogWarning("Workshift with ID {Id} not found for update", id);
            return NotFound();
        }

        if (workshift.StartTime != default && workshift.StartTime != existing.StartTime)
            existing.StartTime = workshift.StartTime;
        if (workshift.EndTime != default && workshift.EndTime != existing.EndTime)
            existing.EndTime = workshift.EndTime;
        if (workshift.Therapist != null && workshift.Therapist != existing.Therapist)
            existing.Therapist = workshift.Therapist;
        if (workshift.Practice != null && workshift.Practice != existing.Practice)
            existing.Practice = workshift.Practice;

        _context.SaveChanges();
        _logger.LogInformation("Updated workshift: {WorkshiftJson}", JsonSerializer.Serialize(existing));
        return Ok(new { Message = "Workshift updated successfully", Workshift = existing.ToDTO() });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var workshift = _context.Workshifts.Find(id);
        if (workshift == null)
        {
            _logger.LogWarning("Workshift with ID {Id} not found for deletion", id);
            return NotFound();
        }
        _context.Workshifts.Remove(workshift);
        _context.SaveChanges();
        _logger.LogInformation("Deleted workshift with ID {Id}: {WorkshiftJson}", id, JsonSerializer.Serialize(workshift));
        return Ok(new { Message = "Workshift deleted successfully", Workshift = workshift.ToDTO() });
    }

    public Workshift ToWorkshift(WorkshiftSummaryDTO dto)
    {
        return new Workshift
        {
            Id = dto.Id,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Therapist = _context.Therapists.Find(dto.TherapistId) ?? throw new ArgumentException($"Therapist with ID {dto.TherapistId} not found"),
            Practice = _context.Practices.Find(dto.PracticeId) ?? throw new ArgumentException($"Practice with ID {dto.PracticeId} not found")
        };
    }
}
