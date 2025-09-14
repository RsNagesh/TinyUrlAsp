using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using static IndexModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("TinyUrlApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7120/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var options = new System.Text.Json.JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();  
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.Run();
