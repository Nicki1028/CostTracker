using CostApi_EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CostApi_EF.Data;

// DbContext 是 EF Core 的核心，代表「跟資料庫的一個連線工作階段」，
// 每個 DbSet<T> 對應資料庫裡的一張表。
public class CostDbContext : DbContext
{
    public CostDbContext(DbContextOptions<CostDbContext> options) : base(options)
    {
    }

    public DbSet<CostItem> Costs => Set<CostItem>();
}
