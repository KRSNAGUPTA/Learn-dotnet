using Microsoft.EntityFrameworkCore;
using todo_dotnet.Models;
using todo_dotnet.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapGet("/", () =>
{
    return "Just Hello";
});

app.MapGet("/api/todos", async (AppDbContext db) =>
{
    var data = await db.TodoItems.ToListAsync();

    return new
    {
        message="Fetched all!",
        data
    };
});

app.MapPost("/api/todos", async(AppDbContext db) =>
{
    
});

app.Run();