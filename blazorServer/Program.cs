using blazorServer.Data;
using blazorServer.Models;
using blazorServer.Services;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfigurationSection? employeeSection = builder.Configuration.GetSection(nameof(EmployeeStoreDatabaseSettings));
builder.Services.Configure<EmployeeStoreDatabaseSettings>(employeeSection);

builder.Services.AddSingleton(sp =>
sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<IEmployeeStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
new MongoClient(builder.Configuration.GetValue<string>("EmployeeStoreDatabaseSettings:ConnectionString")));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My api", Version = "v1" });
//});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddIgniteUIBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI();


    // https://www.youtube.com/watch?v=iWTdJ1IYGtg
    // 
    //app.UseSwagger(c =>
    //{
    //    c.RouteTemplate = "mycoolapi/swagger/blazorServer/swagger.json";
    //});
    //app.UseSwaggerUI(c =>
    //{
    //    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "blazorServer v1");
    //    c.SwaggerEndpoint("/mycoolapi/swagger/v1/swagger.json", "My Cool API V1");
    //    c.RoutePrefix = "mycoolapi/swagger";
    //});
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();
