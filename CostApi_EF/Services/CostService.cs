using CostApi_EF.Data;
using CostApi_EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CostApi_EF.Services;

// 原本的 CsvService 讀寫檔案邏輯，改成用 EF Core 操作資料庫。
// 介面（ICostService）完全沒變，所以 CostsController 幾乎不用改。
public class CostService : ICostService
{
    private readonly CostDbContext _db;

    public CostService(CostDbContext db)
    {
        _db = db;
    }

    public void AddCost(CostItem item)
    {
        item.Id = 0; // Id 由資料庫自動編號，強制歸零避免被誤傳成更新既有資料
        _db.Costs.Add(item);
        _db.SaveChanges();
    }

    public List<CostItem> GetCosts(
        DateOnly start,
        DateOnly end,
        string incomexpense,
        string item,
        string paymethod)
    {
        var query = _db.Costs.Where(c => c.Datetime >= start && c.Datetime <= end);

        if (incomexpense != "全部")
        {
            query = query.Where(c => c.Incomexpense == incomexpense);
        }

        if (item != "全部")
        {
            query = query.Where(c => c.Item == item);
        }

        if (paymethod != "全部")
        {
            query = query.Where(c => c.Paymethod == paymethod);
        }

        return query.OrderByDescending(c => c.Datetime).ToList();
    }
}
