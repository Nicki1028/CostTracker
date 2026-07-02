using System.Globalization;
using System.Text;
using CostApi.Models;
using Microsoft.Data.SqlClient;

namespace CostApi.Services;

public class CostService : ICostService
{
    //CSV版
    //private readonly string _basePath;

    //public CostService(IConfiguration configuration)
    //{
    //    // appsettings.json 裡設定 "FilePath"，對應原本 App.config 的 filePath
    //    _basePath = configuration["FilePath"] ?? Path.Combine(AppContext.BaseDirectory, "Data");
    //    Directory.CreateDirectory(_basePath);
    //}

    //private string GetFilePath(DateOnly date)
    //{
    //    var dayFolder = Path.Combine(_basePath, date.ToString("yyyy-MM-dd"));
    //    Directory.CreateDirectory(dayFolder);
    //    return Path.Combine(dayFolder, $"{date:yyyy-MM-dd}.csv");
    //}

    //public void AddCost(CostItem item)
    //{
    //    if (!DateOnly.TryParse(item.Datetime, out var date))
    //    {
    //        throw new ArgumentException("日期格式錯誤，請使用 yyyy-MM-dd");
    //    }

    //    var path = GetFilePath(date);
    //    var isNewFile = !File.Exists(path);

    //    using var writer = new StreamWriter(path, append: true, Encoding.UTF8);
    //    if (isNewFile)
    //    {
    //        writer.WriteLine("Datetime,Incomexpense,Item,Detail,Paymethod,Money");
    //    }
    //    writer.WriteLine(ToCsvLine(item));
    //}

    //public List<CostItem> GetCosts(
    //    DateOnly start,
    //    DateOnly end,
    //    string incomexpense,
    //    string item,
    //    string paymethod)
    //{
    //    var result = new List<CostItem>();

    //    for (var date = start; date <= end; date = date.AddDays(1))
    //    {
    //        var path = GetFilePath(date);
    //        if (!File.Exists(path))
    //        {
    //            continue;
    //        }

    //        var lines = File.ReadAllLines(path, Encoding.UTF8).Skip(1); // 跳過表頭
    //        foreach (var line in lines)
    //        {
    //            if (string.IsNullOrWhiteSpace(line))
    //            {
    //                continue;
    //            }

    //            var cost = FromCsvLine(line);

    //            if (incomexpense != "全部" && cost.Incomexpense != incomexpense) continue;
    //            if (item != "全部" && cost.Item != item) continue;
    //            if (paymethod != "全部" && cost.Paymethod != paymethod) continue;

    //            result.Add(cost);
    //        }
    //    }

    //    return result.OrderByDescending(c => c.Datetime).ToList();
    //}

    //private static string ToCsvLine(CostItem item)
    //{
    //    string Escape(string s) => s.Contains(',') || s.Contains('"')
    //        ? $"\"{s.Replace("\"", "\"\"")}\""
    //        : s;

    //    return string.Join(",",
    //        Escape(item.Datetime),
    //        Escape(item.Incomexpense),
    //        Escape(item.Item),
    //        Escape(item.Detail),
    //        Escape(item.Paymethod),
    //        item.Money.ToString(CultureInfo.InvariantCulture));
    //}

    //private static CostItem FromCsvLine(string line)
    //{
    //    var parts = SplitCsvLine(line);
    //    return new CostItem
    //    {
    //        Datetime = parts[0],
    //        Incomexpense = parts[1],
    //        Item = parts[2],
    //        Detail = parts[3],
    //        Paymethod = parts[4],
    //        Money = decimal.Parse(parts[5], CultureInfo.InvariantCulture)
    //    };
    //}

    //// 簡易 CSV 逐字元解析，可處理欄位內含逗號、雙引號的情況（沒有依賴 CSV1）
    //private static string[] SplitCsvLine(string line)
    //{
    //    var result = new List<string>();
    //    var current = new StringBuilder();
    //    var inQuotes = false;

    //    for (var i = 0; i < line.Length; i++)
    //    {
    //        var c = line[i];

    //        if (inQuotes)
    //        {
    //            if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
    //            {
    //                current.Append('"');
    //                i++;
    //            }
    //            else if (c == '"')
    //            {
    //                inQuotes = false;
    //            }
    //            else
    //            {
    //                current.Append(c);
    //            }
    //        }
    //        else
    //        {
    //            if (c == '"')
    //            {
    //                inQuotes = true;
    //            }
    //            else if (c == ',')
    //            {
    //                result.Add(current.ToString());
    //                current.Clear();
    //            }
    //            else
    //            {
    //                current.Append(c);
    //            }
    //        }
    //    }

    //    result.Add(current.ToString());
    //    return result.ToArray();
    //}

    private readonly string _connectionString;  //SQLServer

    public CostService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("找不到資料庫連線字串");
    }

    public void AddCost(CostItem item)
    {
        if (!DateOnly.TryParse(item.Datetime, out var date))
        {
            throw new ArgumentException("日期格式錯誤，請使用 yyyy-MM-dd");
        }

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var sql = @"
            INSERT INTO CostItems
            (Datetime, Incomexpense, Item, Detail, Paymethod, Money)
            VALUES
            (@Datetime, @Incomexpense, @Item, @Detail, @Paymethod, @Money)
        ";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Datetime", date.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@Incomexpense", item.Incomexpense);
        cmd.Parameters.AddWithValue("@Item", item.Item);
        cmd.Parameters.AddWithValue("@Detail", item.Detail);
        cmd.Parameters.AddWithValue("@Paymethod", item.Paymethod);
        cmd.Parameters.AddWithValue("@Money", item.Money);

        cmd.ExecuteNonQuery();
    }

    public List<CostItem> GetCosts(
        DateOnly start,
        DateOnly end,
        string incomexpense,
        string item,
        string paymethod)
    {
        var result = new List<CostItem>();

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var sql = @"
            SELECT Datetime, Incomexpense, Item, Detail, Paymethod, Money
            FROM CostItems
            WHERE Datetime >= @Start
              AND Datetime <= @End
              AND (@Incomexpense = N'全部' OR Incomexpense = @Incomexpense)
              AND (@Item = N'全部' OR Item = @Item)
              AND (@Paymethod = N'全部' OR Paymethod = @Paymethod)
            ORDER BY Datetime DESC
        ";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Start", start.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@End", end.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@Incomexpense", incomexpense);
        cmd.Parameters.AddWithValue("@Item", item);
        cmd.Parameters.AddWithValue("@Paymethod", paymethod);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new CostItem
            {
                Datetime = DateOnly.FromDateTime(reader.GetDateTime(0)).ToString("yyyy-MM-dd"),
                Incomexpense = reader.GetString(1),
                Item = reader.GetString(2),
                Detail = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Paymethod = reader.GetString(4),
                Money = reader.GetDecimal(5)
            });
        }
        return result;

        //EF版
        //private readonly CostDbContext _context;

        //public CostService(CostDbContext context)
        //{
        //    _context = context;
        //}

        //public void AddCost(CostItem item)
        //{
        //    _context.CostItems.Add(item);
        //    _context.SaveChanges();
        //}

        //public List<CostItem> GetCosts(
        //    DateOnly start,
        //    DateOnly end,
        //    string incomexpense,
        //    string item,
        //    string paymethod)
        //{
        //    var query = _context.CostItems.AsQueryable();

        //    query = query.Where(x =>
        //        string.Compare(x.Datetime, start.ToString("yyyy-MM-dd")) >= 0 &&
        //        string.Compare(x.Datetime, end.ToString("yyyy-MM-dd")) <= 0);

        //    if (incomexpense != "全部")
        //    {
        //        query = query.Where(x => x.Incomexpense == incomexpense);
        //    }

        //    if (item != "全部")
        //    {
        //        query = query.Where(x => x.Item == item);
        //    }

        //    if (paymethod != "全部")
        //    {
        //        query = query.Where(x => x.Paymethod == paymethod);
        //    }

        //    return query
        //        .OrderByDescending(x => x.Datetime)
        //        .ToList();
        //}
    }
}
