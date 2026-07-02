using CostApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CostApi.Data;

public class CostDbContext : DbContext
{
    public CostDbContext(DbContextOptions<CostDbContext> options)
        : base(options)
    {
    }

    public DbSet<CostItem> CostItems { get; set; }
}