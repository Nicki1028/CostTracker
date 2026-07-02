using CostApi.Models;

namespace CostApi.Services;

public interface ICostService
{
    void AddCost(CostItem item);

    List<CostItem> GetCosts(
        DateOnly start,
        DateOnly end,
        string incomexpense,
        string item,
        string paymethod);
}
