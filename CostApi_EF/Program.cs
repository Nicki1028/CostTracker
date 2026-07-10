using CostApi_EF.Data;
using CostApi_EF.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 註冊 DbContext，連線字串從 appsettings.json 的 ConnectionStrings:CostApiDb 讀取
builder.Services.AddDbContext<CostDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CostApiDb")));

// 這裡從 AddSingleton 改成 AddScoped：
// DbContext 每個請求要有自己獨立的一份（不能像檔案路徑那樣整個應用程式共用一個實體），
// 所以依賴它的 CostService 生命週期也要跟著改成 Scoped（每個 HTTP 請求一份）。
builder.Services.AddScoped<ICostService, CostService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Angular 開發環境預設跑在 http://localhost:4200，
// 這支 API 若跑在 http://localhost:5000，瀏覽器會擋跨來源請求，所以要開 CORS。
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 開發環境啟動時自動套用 migrations，資料庫/資料表不存在會自動建立。
// 正式環境通常不會這樣做（改用 dotnet ef database update 手動控管），
// 但開發階段先求方便。
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CostDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularDev");
app.UseAuthorization();
app.MapControllers();

app.Run();
