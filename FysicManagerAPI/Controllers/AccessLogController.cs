using Microsoft.AspNetCore.Mvc;
using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using FysicManagerAPI.Models.DTO_s;
using Microsoft.EntityFrameworkCore;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/accesslog")]
public class AccessLogController : ControllerBase
{
    private readonly ILogger<AccessLogController> _logger;
    private readonly AppDbContext _context;

    public AccessLogController(ILogger<AccessLogController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            // Limit page size to prevent abuse
            pageSize = Math.Min(pageSize, 1000);
            
            var query = _context.AccessLogs
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var logs = await query.Select(a => a.ToDTO()).ToListAsync();
            var totalCount = await _context.AccessLogs.CountAsync();

            var response = new
            {
                Data = logs,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access logs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var log = await _context.AccessLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound($"Access log with id {id} not found");
            }

            return Ok(log.ToDTO());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access log {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int hours = 24, [FromQuery] int limit = 100)
    {
        try
        {
            limit = Math.Min(limit, 1000); // Prevent abuse
            var since = DateTime.UtcNow.AddHours(-Math.Abs(hours));

            var logs = await _context.AccessLogs
                .Where(a => a.Timestamp >= since)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .Select(a => a.ToDTO())
                .ToListAsync();

            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent access logs");
            return StatusCode(500, "Internal server error");
        }
    }    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int hours = 24)
    {
        try
        {
            var since = DateTime.UtcNow.AddHours(-Math.Abs(hours));

            var stats = await _context.AccessLogs
                .Where(a => a.Timestamp >= since)
                .GroupBy(a => new { a.HttpMethod, a.RequestPath })
                .Select(g => new
                {
                    Method = g.Key.HttpMethod,
                    Path = g.Key.RequestPath,
                    Count = g.Count(),
                    AvgResponseTime = g.Average(x => x.ResponseTimeMs),
                    SuccessRate = (double)g.Count(x => x.StatusCode >= 200 && x.StatusCode < 400) / g.Count() * 100
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access log stats");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? ip, [FromQuery] string? path, [FromQuery] string? method, [FromQuery] int hours = 24)
    {
        try
        {
            var since = DateTime.UtcNow.AddHours(-Math.Abs(hours));
            var query = _context.AccessLogs.Where(a => a.Timestamp >= since);

            if (!string.IsNullOrEmpty(ip))
            {
                query = query.Where(a => a.IpAddress != null && a.IpAddress.Contains(ip));
            }

            if (!string.IsNullOrEmpty(path))
            {
                query = query.Where(a => a.RequestPath.Contains(path));
            }

            if (!string.IsNullOrEmpty(method))
            {
                query = query.Where(a => a.HttpMethod.ToLower() == method.ToLower());
            }

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Take(500) // Limit results
                .Select(a => a.ToDTO())
                .ToListAsync();

            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching access logs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("cleanup")]
    public async Task<IActionResult> Cleanup([FromQuery] int olderThanDays = 90)
    {
        try
        {
            // Safety check - don't allow deleting logs less than 7 days old
            if (olderThanDays < 7)
            {
                return BadRequest("Cannot delete logs newer than 7 days");
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var logsToDelete = await _context.AccessLogs
                .Where(a => a.Timestamp < cutoffDate)
                .ToListAsync();

            _context.AccessLogs.RemoveRange(logsToDelete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} access logs older than {Days} days", logsToDelete.Count, olderThanDays);
            return Ok(new { DeletedCount = logsToDelete.Count, CutoffDate = cutoffDate });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up access logs");
            return StatusCode(500, "Internal server error");
        }
    }
}
