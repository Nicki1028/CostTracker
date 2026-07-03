# CostTracker

## 技術棧
前端：Angular（Standalone Components）、TypeScript
後端：ASP.NET Core Web API（.NET 8）
資料庫：SQL Server
版本控制：Git / GitHub

## 專案架構
CostTracker/
├── frontend/                  # Angular 前端
│   └── src/app/
│       ├── app.ts              # 主要元件，管理畫面狀態
│       ├── services/
│       │   ├── cost.service.ts    # 負責與後端API溝通
│       └── models/
│           ├── cost-item.ts
│           └── chart-data.ts
└── backend/                   # ASP.NET Core Web API
    └── CostApi/
        ├── Controllers/
        │   └── CostsController.cs  # 負責與前端溝通
        ├── Services/
        │   └── CostService.cs      # 業務邏輯層
        │   └── ICostService.cs      
        └── Models/
            └── CostItem.cs
            
## 專案畫面範例
## Add Expense
![Add](images/addcost.png)

## Search
![Search](images/search.png)

## REST API
![Swagger](images/Swagger.png)

## Database
![Database](images/database.png)
