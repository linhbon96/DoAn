using Microsoft.AspNetCore.Mvc;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;
using OfficeOpenXml;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("SalesReport")]
    public async Task<IActionResult> GetSalesReport()
    {
        var salesData = await _context.Tickets
                                      .Include(t => t.Showtime)
                                      .ThenInclude(s => s.Movie)
                                      .GroupBy(t => t.Showtime.Movie.Title)
                                      .Select(g => new
                                      {
                                          MovieTitle = g.Key,
                                          TicketsSold = g.Count(),
                                          TotalRevenue = g.Sum(t => t.Price)
                                      }).ToListAsync();

        return Ok(salesData);
    }

    [HttpGet("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var salesData = await GetSalesReportData(); // Helper function to get data

        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add("Sales Report");
            worksheet.Cells.LoadFromCollection(salesData, true);
            package.Save();
        }
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
    }

    private async Task<IEnumerable<SalesData>> GetSalesReportData()
    {
        return await _context.Tickets
                             .Include(t => t.Showtime)
                             .ThenInclude(s => s.Movie)
                             .GroupBy(t => t.Showtime.Movie.Title)
                             .Select(g => new SalesData
                             {
                                 MovieTitle = g.Key,
                                 TicketsSold = g.Count(),
                                 TotalRevenue = g.Sum(t => t.Price)
                             }).ToListAsync();
    }
}

public class SalesData
{
    public string MovieTitle { get; set; }
    public int TicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}
