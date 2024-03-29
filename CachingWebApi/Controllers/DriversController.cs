using CachingWebApi.Data;
using CachingWebApi.Models;
using CachingWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CachingWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController : ControllerBase
{

    private readonly ILogger<DriversController> _logger;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _context;
    public DriversController(ILogger<DriversController> logger, ICacheService cacheService, AppDbContext context)
    {
        _cacheService = cacheService;
        _context = context;
        _logger = logger;
    }

    [HttpGet("drivers")]
    public async Task<IActionResult> Get()
    {
        var cacheData =_cacheService.GetData<IEnumerable<Driver>>("drivers");
        if(cacheData !=null && cacheData.Count()>0)
            return Ok(cacheData);

        cacheData=await _context.Drivers.ToListAsync();

        var expirtyTime= DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<IEnumerable<Driver>>("drivers",cacheData,expirtyTime);

        return Ok(cacheData);
    }

    [HttpPost("AddDriver")]
    public async Task<IActionResult> Post(Driver driver)
    {
        var addedObj= await _context.Drivers.AddAsync(driver);
        var expiryTime=DateTimeOffset.Now.AddSeconds(30);

        _cacheService.SetData<Driver>($"driver{driver.Id}", addedObj.Entity, expiryTime);

        await _context.SaveChangesAsync();

        return Ok(addedObj.Entity);
    }


    [HttpDelete("DeleteDriver")]
    public async Task<IActionResult> Delete(int id)
    {
        var exist= await _context.Drivers.FirstOrDefaultAsync(x=>x.Id==id);

        if (exist != null)
        {
            _context.Remove(exist);
            _cacheService.RemoveData($"driver{id}");
            await _context.SaveChangesAsync();

            return NoContent();
        }
        return NotFound();
    }
}
