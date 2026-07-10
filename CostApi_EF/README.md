# CostApi — SQL Server 版本

這份是把原本 CSV 檔案儲存的版本，改成用 **Entity Framework Core + SQL Server (LocalDB)** 存資料。

## 跟 CSV 版本的差異對照

| CSV 版本 | SQL 版本 | 說明 |
|---|---|---|
| `Services/CostService.cs` 自己讀寫檔案 | `Services/CostService.cs` 改用 `CostDbContext` 操作資料庫 | `ICostService` 介面完全沒變，`CostsController` 幾乎不用改 |
| 沒有 `Data/` 資料夾 | 新增 `Data/CostDbContext.cs` | EF Core 的核心，代表「跟資料庫的連線工作階段」 |
| `CostItem.Datetime` 是 `string` | 改成 `DateOnly` | 資料庫存成真正的日期型別，能排序、比大小；JSON 格式對前端來說沒有變化，還是 `"2026-06-30"` 這種字串 |
| `CostItem` 沒有 Id | 新增 `Id`（int，自動編號） | 資料庫的表一定要有主鍵，CSV 版本靠檔案位置區分資料列，資料庫不行 |
| `appsettings.json` 的 `FilePath` | 改成 `ConnectionStrings:CostApiDb` | 連線字串，指定要連去哪個資料庫 |
| `AddSingleton<ICostService, CostService>()` | 改成 `AddScoped<ICostService, CostService>()` | 見下方說明 |

### 為什麼 `AddSingleton` 要改成 `AddScoped`

`DbContext` 內部會追蹤目前這次查詢/存檔的狀態（EF Core 官方文件明確說 `DbContext` 不是執行緒安全的），**不能像檔案路徑字串那樣整個應用程式共用同一份**。標準做法是每個 HTTP 請求都給一份全新的 `DbContext`，這就是 `AddScoped` 的意思——「同一個請求內共用一份，不同請求之間各自獨立」。因為 `CostService` 建構子依賴 `CostDbContext`，所以 `CostService` 的生命週期也要跟著改成 `AddScoped`，不能再用 `AddSingleton`。

## 執行前的準備

### 1. 還原套件
```bash
dotnet restore
```

### 2. 安裝 EF Core 的命令列工具（第一次用才需要，全域只要裝一次）
```bash
dotnet tool install --global dotnet-ef
```
如果之前有裝過舊版，改用：
```bash
dotnet tool update --global dotnet-ef
```

### 3. 建立 Migration（資料庫結構的版本紀錄）
```bash
dotnet ef migrations add InitialCreate
```
這會在專案裡產生一個 `Migrations` 資料夾，記錄「資料表要長什麼樣子」，是根據 `CostItem.cs` 的屬性自動產生的。

### 4. 建立資料庫
```bash
dotnet ef database update
```
這一步才會真的去 SQL Server LocalDB 建出資料庫和資料表。

**如果你嫌每次都要手動下指令麻煩**：`Program.cs` 裡已經加了 `db.Database.Migrate()`，開發環境啟動時會自動套用最新的 migration，資料庫/資料表不存在會自動建立——但**前提是你至少要手動跑過一次 `dotnet ef migrations add`**，讓 migration 檔案先產生出來，之後啟動就不用再手動 `database update` 了。

### 5. 啟動
```bash
dotnet run
```

## 用 SQL Server Object Explorer 檢查資料庫（Visual Studio）

Visual Studio 左側「檢視」→「SQL Server 物件總管」，展開 `(localdb)\MSSQLLocalDB` → `Databases` → `CostApiDb` → `Tables`，應該會看到一張 `Costs` 表，欄位是 `Id, Datetime, Incomexpense, Item, Detail, Paymethod, Money`。可以直接在這裡右鍵「檢視資料」，確認 Angular 新增的資料有沒有真的寫進去。

## 之後 Schema 有改動時（例如加欄位）

1. 改 `Models/CostItem.cs`（例如加一個新屬性）
2. 產生新的 migration：
   ```bash
   dotnet ef migrations add 這次改動的說明
   ```
3. 套用到資料庫：
   ```bash
   dotnet ef database update
   ```
（如果 `Program.cs` 有 `db.Database.Migrate()`，重新啟動也會自動套用，不一定要手動下第 3 步。）

## 其他保留不變的部分

- `Controllers/CostsController.cs` 的路由、`GET`/`POST` 兩支 API 完全沒變
- `Program.cs` 的 CORS 設定沒變，還是只允許 `http://localhost:4200`
- Angular 前端**完全不用改**——`cost.service.ts`、`cost-item.ts` 都不用動，因為 API 收發的 JSON 格式沒有變化（`Id` 這個新欄位前端不用管，新增時不用傳，查詢回來的資料裡會多一個 `id` 欄位，Angular 那邊沒用到也不會出錯）
