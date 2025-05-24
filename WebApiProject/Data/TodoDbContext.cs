using Microsoft.EntityFrameworkCore;

namespace WebApiProject.Data;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var trackedTodos=ChangeTracker.Entries<Todo>().Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var todoEntry in trackedTodos)
        {
            var entity=todoEntry.Entity;
            switch (todoEntry.State)
            {
                case EntityState.Added:
                    entity.CreatedAtUTC=DateTime.UtcNow;
                    entity.CreatedBy = "cool user";
                    break;
                case EntityState.Modified:
                    entity.LastModifiedAtUTC=DateTime.UtcNow;
                    entity.LastModifiedBy = "cool user";
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}   