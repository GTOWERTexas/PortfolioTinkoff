using Tinkoff.InvestApi.V1;
using Tinkoff.InvestApi;
using PortfolioTinkoff.Services;
using PortfolioTinkoff.Models;
using PortfolioTinkoff.Models.OperationsClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddInvestApiClient((_, settings)=>settings.AccessToken = "your token");
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddTransient<IMainTable, MainTable>();
builder.Services.AddScoped<OperationTable>(/*sp => SessionOperationTable.GetOperationTable(sp)*/);
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<OperationService>();
builder.Services.AddTransient<InstrumentService>();
builder.Services.AddTransient<QuatationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

//app.UseAuthorization();
app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
