namespace CostApi.Models;

// 對應 Angular 的 cost-item.ts
// 注意：Money 這裡改成 decimal，原本 C# WinForms 版本是 string，
// 因為前端 Angular 的 CostItem.money 是 number，用 decimal 才能直接對上、也才能做加總運算。
public class CostItem
{
    public string Datetime { get; set; } = "";       // 格式 yyyy-MM-dd，對齊 <input type="date">
    public string Incomexpense { get; set; } = "";    // "收入" 或 "支出"
    public string Item { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Paymethod { get; set; } = "";
    public decimal Money { get; set; }
}


//public class CostItem
//{
//    public int Id { get; set; }

//    public string Datetime { get; set; } = "";

//    public string Incomexpense { get; set; } = "";

//    public string Item { get; set; } = "";

//    public string Detail { get; set; } = "";

//    public string Paymethod { get; set; } = "";

//    public decimal Money { get; set; }
//}
