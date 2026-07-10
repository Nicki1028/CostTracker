using CostApi_EF.Models;

namespace CostApi_EF.Services;

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
