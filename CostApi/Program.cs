using CostApi.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ICostService, CostService>();
//builder.Services.AddScoped<ICostService, CostService>();
////builder.Services.AddDbContext<CostDbContext>(options =>
//    options.UseSqlServer(
//    builder.Configuration.GetConnectionString("DefaultConnection")));

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


//var app = builder.Build();
//app.UseCors(policy =>
//    policy.AllowAnyOrigin()
//          .AllowAnyHeader()
//          .AllowAnyMethod());
//app.MapControllers();
//app.Run();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularDev");
app.UseAuthorization();
app.MapControllers();
app.Run();
