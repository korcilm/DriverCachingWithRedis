using Microsoft.EntityFrameworkCore;
using CachingWebApi.Models;

namespace CachingWebApi.Data;

public class AppDbContext:DbContext
{
    public DbSet<Driver> Drivers {get;set;}
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
}

