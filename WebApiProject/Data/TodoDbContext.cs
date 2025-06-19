using Microsoft.EntityFrameworkCore;

namespace WebApiProject.Data;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }

    
}   