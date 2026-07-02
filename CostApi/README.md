# CostApi 使用說明

這是根據你原本 C# WinForms 記帳系統的邏輯，改寫成 ASP.NET Core Web API，
專門對接你已經寫好的 Angular 前端。

## 跟原本 WinForms 專案的對應關係

| 原本 (WinForms)                          | 現在 (Web API)                          | 說明 |
|------------------------------------------|------------------------------------------|------|
| `CostItem.cs`                            | `Models/CostItem.cs`                     | `Money` 從 `string` 改成 `decimal`，對齊 Angular 的 `money: number` |
| `CsvService.WriteCsv()` / `ReadCsv()`    | `Services/CostService.cs`                | 沿用「每天一個資料夾一個 CSV」的儲存方式，只是改成不依賴 `CSV1` 這個自製套件 |
| `CsvService.Groupbydatas()`              | （不需要了）                              | 分組加總的邏輯已經搬到 Angular 前端的 `generateChart()` 直接處理 `records` |
| `Director.cs` / `Chart/*Builder.cs`      | （不需要了）                              | 這些是 WinForms 專用的圖表繪製元件，瀏覽器用不到，Angular 端已經自己用 CSS/SVG 畫圖表了 |
| 表單 `記一筆.cs` 的送出邏輯               | `Controllers/CostsController.cs` 的 `POST /api/costs` | |
| 表單 `帳戶.cs` / `圖表分析.cs` 的查詢邏輯 | `Controllers/CostsController.cs` 的 `GET /api/costs` | |
| `App.config` 的 `filePath`               | `appsettings.json` 的 `FilePath`         | |

沒有搬過來的部分：`記事本.cs`、`Navbar.cs`、`PictureForm.cs`、收據/產品圖片欄位（`Picture1Path`/`Picture2Path`）—
因為你目前 Angular 的畫面只有「新增記帳」「查詢紀錄」兩頁，沒有這些功能，先不處理，之後有需要再加。

## 如何啟動

1. 用 Visual Studio 或 `dotnet` CLI 開啟這個資料夾（`CostApi.csproj`）
2. 確認 `appsettings.json` 的 `FilePath` 是你電腦上想存放 CSV 的資料夾（記得資料夾要存在或有建立權限）
3. 執行：
   ```
   dotnet run
   ```
   或在 Visual Studio 按 F5。預設會跑在 `http://localhost:5000`（在 `Properties/launchSettings.json` 設定的），
   跟你 Angular `cost.service.ts` 裡寫死的 `apiUrl = 'http://localhost:5000/api/costs'` 剛好對上。
4. 瀏覽器打開 `http://localhost:5000/swagger` 可以直接測試 API，不用等前端寫好就能先試。

## 兩支 API

- `GET /api/costs?start=2026-06-01&end=2026-06-30&incomexpense=全部&item=全部&paymethod=全部`
  對應 `searchRecords()`，回傳 `CostItem[]`
- `POST /api/costs`（body 是一筆 `CostItem` 的 JSON）
  對應 `addRecord()`，成功回傳 204 No Content

## 執行前務必檢查的地方

1. **CORS**：`Program.cs` 裡只開放 `http://localhost:4200`（Angular 預設開發 port）。
   如果你的 Angular 是跑在別的 port，記得改 `AllowAngularDev` policy 裡的網址。
2. **日期格式**：這支 API 用 `yyyy-MM-dd`（跟 `<input type="date">` 一致）。
   你原本 WinForms 寫入 CSV 用的是 `yyyy/MM/dd`，兩者不同，如果你有舊資料要沿用，
   要先批次轉換日期格式，不然 `DateOnly.TryParse` 可能讀不到。
3. **Money 的正負號**：這支 API 不會自動把「支出」轉成負數，前端輸入多少就存多少。
   如果你希望支出在畫面上顯示負數（紅字 `-$85` 那種），可以在 `AddCost()` 裡依 `Incomexpense` 自動轉正負號，
   或維持現況、純靠前端的 `incomexpense` 欄位判斷顏色（目前你的 `app.html` 表格顏色綁定其實有點小 bug，
   `[class.income]="item.incomexpense"` 這樣寫只要 `incomexpense` 有值就一定是 true，之後可以再一起修）。
4. **CSV 檔案儲存方式**：這是直接搬原本桌面版的做法（檔案系統存 CSV），
   對單機測試沒問題，但正式上線、多人同時新增記帳時，檔案讀寫可能會有 race condition
   （兩個請求同時寫同一天的 CSV）。如果之後要正式部署，建議改用 SQLite 或 SQL Server + Entity Framework Core，
   我可以再幫你做這個遷移。
