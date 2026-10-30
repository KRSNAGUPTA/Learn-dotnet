using Microsoft.EntityFrameworkCore;
using todo_dotnet.Models;
using todo_dotnet.Data;
using Microsoft.AspNetCore.Components.Forms;
using SQLitePCL;

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
        message = "Fetched all!",
        data
    };
});


app.MapGet("/api/todos/{id:int}", async (int id, AppDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);

    if(todo is null)
    {
        return Results.NotFound(new {error=$"Not found todo with id {id}"});
    }

    return Results.Ok(new {message=$"Fetched todo with id {id}", data=todo});
});
app.MapPost("/api/todos", async (CreateTodoInput req, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.title))
    {
        return Results.BadRequest(new { error = "Title is required" });
    }
    try
    {
        var todo = new TodoItems
        {
            Title = req.title,
            Description = req.description ?? string.Empty
        };
        db.TodoItems.Add(todo);
        await db.SaveChangesAsync();
        return Results.Created($"/api/todo/{todo.Id}", todo);

    }catch(Exception e)
    {
        return Results.InternalServerError(new {error = "Internal Server Error", deatils=e.Message, inner= e.InnerException?.Message});
    }

});


app.MapDelete("/api/todos/{id:int}", async(int id, AppDbContext db) =>
{   
    var todo = await db.TodoItems.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound(new {error= $"Id {id} Not found"});
    }
    db.TodoItems.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapPatch("/api/todos/{id:int}", async (int id, PatchTodo req, AppDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if(todo is null)
    {
        return Results.NotFound(new {error = $"No todo found for {id}"});
    }
    if(req.title is not null)
    {
    todo.Title = req.title ?? todo.Title;
    }
    
    if(req.description is not null)
    {
    todo.Description = req.description ?? todo.Description;
        
    }
    if(req.isCompleted is not null)
    {
    todo.IsCompleted = req.isCompleted.Value;
    }

    await db.SaveChangesAsync();
    return Results.Ok(new {message = "Done", data= todo});

});

app.Run();
public record CreateTodoInput(string title, string description);
public record PatchTodo(string? title, string? description, bool? isCompleted);