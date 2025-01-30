using FinalKPLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Doc> Docs { get; set; }
    public DbSet<VisibilityArea> VisibilityAreas { get; set; }
    public DbSet<UserVisibilityArea> UserVisibilityAreas { get; set; }
    public DbSet<DocVisibilityArea> DocVisibilityAreas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserVisibilityArea>()
            .HasKey(uv => new { uv.UserId, uv.VisibilityAreaId });

        modelBuilder.Entity<DocVisibilityArea>()
            .HasKey(dv => new { dv.DocId, dv.VisibilityAreaId });
    }
}