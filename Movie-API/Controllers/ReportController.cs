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

        var itemSalesData = await _context.ItemOrders
                                          .Include(io => io.Item)
                                          .GroupBy(io => io.Item.Name)
                                          .Select(g => new
                                          {
                                              ItemName = g.Key,
                                              QuantitySold = g.Sum(io => io.Quantity),
                                              TotalRevenue = g.Sum(io => io.Quantity * io.Item.Price)
                                          }).ToListAsync();

        return Ok(new { MovieSales = salesData, ItemSales = itemSalesData });
    }

    [HttpGet("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var salesData = await GetSalesReportData(); // Helper function to get data

        var stream = new MemoryStream();
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add("Sales Report");
            worksheet.Cells["A1"].LoadFromCollection(salesData.MovieSales, true);
            worksheet.Cells["D1"].LoadFromCollection(salesData.ItemSales, true);
            package.Save();
        }
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
    }

    private async Task<SalesReportData> GetSalesReportData()
    {
        var movieSales = await _context.Tickets
                                       .Include(t => t.Showtime)
                                       .ThenInclude(s => s.Movie)
                                       .GroupBy(t => t.Showtime.Movie.Title)
                                       .Select(g => new SalesData
                                       {
                                           MovieTitle = g.Key,
                                           TicketsSold = g.Count(),
                                           TotalRevenue = g.Sum(t => t.Price)
                                       }).ToListAsync();

        var itemSales = await _context.ItemOrders
                                      .Include(io => io.Item)
                                      .GroupBy(io => io.Item.Name)
                                      .Select(g => new ItemSalesData
                                      {
                                          ItemName = g.Key,
                                          QuantitySold = g.Sum(io => io.Quantity),
                                          TotalRevenue = g.Sum(io => io.Quantity * io.Item.Price)
                                      }).ToListAsync();

        return new SalesReportData { MovieSales = movieSales, ItemSales = itemSales };
    }
}

public class SalesData
{
    public string MovieTitle { get; set; }
    public int TicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class ItemSalesData
{
    public string ItemName { get; set; }
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class SalesReportData
{
    public IEnumerable<SalesData> MovieSales { get; set; }
    public IEnumerable<ItemSalesData> ItemSales { get; set; }
}

