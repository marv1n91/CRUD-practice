using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

public class DataContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=test.db");
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>()
            .HasOne(x => x.User)
            .WithMany(x => x.Notes)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<User> Users => Set<User>();
}
