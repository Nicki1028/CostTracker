# CostTracker

## 技術棧
前端：Angular（Standalone Components）、TypeScript
後端：ASP.NET Core Web API（.NET 8）
資料庫：SQL Server
版本控制：Git / GitHub

## 專案架構
前端（Angular）

檔案 / 資料夾說明frontend/src/app/app.ts主要元件，管理畫面狀態frontend/src/app/services/cost.service.ts負責與後端 API 溝通frontend/src/app/services/chart.service.ts負責圖表資料的分組運算frontend/src/app/models/cost-item.ts記帳資料的型別定義frontend/src/app/models/chart-data.ts圖表資料的型別定義

後端（ASP.NET Core Web API）

檔案 / 資料夾說明backend/CostApi/Controllers/CostsController.cs負責與前端溝通，定義 REST API 路由backend/CostApi/Services/CostService.cs業務邏輯層backend/CostApi/Services/ICostService.cs業務邏輯層的介面backend/CostApi/Data/CostDbContext.csEF Core 資料庫存取入口backend/CostApi/Models/CostItem.cs記帳資料的資料模型


            
## 專案畫面範例
## Add Expense
![Add](images/addcost.png)

## Search
![Search](images/search.png)

## REST API
![Swagger](images/Swagger.png)

## Database
![Database](images/database.png)
