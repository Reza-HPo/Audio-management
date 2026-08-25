using MaktabAhvaz.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaktabAhvaz.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    public DbSet<AudioFile> AudioFiles => Set<AudioFile>();
    public DbSet<Speaker> Speakers => Set<Speaker>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<AudioCategory> AudioCategories => Set<AudioCategory>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Speaker → AudioFile
        modelBuilder.Entity<AudioFile>()
            .HasOne(a => a.Speaker)
            .WithMany()
            .HasForeignKey(a => a.SpeakerId)
            .OnDelete(DeleteBehavior.Restrict);

        // AudioFile ↔ Category
        modelBuilder.Entity<AudioCategory>()
            .HasKey(ac => new { ac.AudioFileId, ac.CategoryId });

        modelBuilder.Entity<AudioCategory>()
            .HasOne(ac => ac.AudioFile)
            .WithMany(a => a.AudioCategories)
            .HasForeignKey(ac => ac.AudioFileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AudioCategory>()
            .HasOne(ac => ac.Category)
            .WithMany(c => c.AudioCategories)
            .HasForeignKey(ac => ac.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Menu → Parent Menu
        modelBuilder.Entity<Menu>()
            .HasOne(m => m.Parent)
            .WithMany()
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // SiteSetting
        modelBuilder.Entity<SiteSetting>()
            .HasIndex(s => s.Key)
            .IsUnique();
    }
}