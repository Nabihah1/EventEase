using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventEase.Data;
using EventEase.Services;
using EventEase.Models;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

//register the blob service 
builder.Services.AddScoped<IBlobService, BlobService>();

//add services to the container 
builder.Services.AddControllersWithViews(); 


builder.Services.AddDbContext<EventEaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventEaseContext") ?? throw new InvalidOperationException("Connection string 'EventEaseContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//azure blob connection string storage 

//var builderss = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var storageConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

// Optional: fallback to appsettings.json if env variable is not set
if (string.IsNullOrEmpty(storageConnectionString))
{
    storageConnectionString = configuration.GetSection("Storage")["ConnectionString"];
}



app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();




