# CostApi 使用說明

## 如何啟動

1. 用 Visual Studio 或 `dotnet` CLI 開啟這個資料夾（`CostApi.csproj`）
2. 確認 `appsettings.json` 的 `FilePath` 是你電腦上想存放 CSV 的資料夾或是建立localDB
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
2. **Money 的正負號**：這支 API 不會自動把「支出」轉成負數，前端輸入多少就存多少。
   如果你希望支出在畫面上顯示負數（紅字 `-$85` 那種），可以在 `AddCost()` 裡依 `Incomexpense` 自動轉正負號，
   或維持現況、純靠前端的 `incomexpense` 欄位判斷顏色（目前你的 `app.html` 表格顏色綁定其實有點小 bug，
   `[class.income]="item.incomexpense"` 這樣寫只要 `incomexpense` 有值就一定是 true，之後可以再一起修）。

