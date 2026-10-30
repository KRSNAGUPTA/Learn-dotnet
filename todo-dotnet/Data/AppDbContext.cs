using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using todo_dotnet.Data;
using todo_dotnet.Models;

namespace todo_dotnet.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}
    public DbSet<TodoItems> TodoItems => Set<TodoItems>();
}