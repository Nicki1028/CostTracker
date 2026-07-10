namespace CostApi_EF.Models;

// 對應 Angular 的 cost-item.ts，但多了 Id（資料庫一定要有主鍵）
public class CostItem
{
    public int Id { get; set; }                      // 主鍵，資料庫自動編號，前端新增時不用傳這個欄位

    public DateOnly Datetime { get; set; }            // 資料庫存成 date 型別，比存字串更適合排序、篩選
    public string Incomexpense { get; set; } = "";    // "收入" 或 "支出"
    public string Item { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Paymethod { get; set; } = "";
    public decimal Money { get; set; }
}
