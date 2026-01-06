using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {

  }

  public DbSet<Organization> Organizations { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<Property> Properties { get; set; }
  public DbSet<Lead> Leads { get; set; }
  public DbSet<Visit> Visits { get; set; }
  public DbSet<InternalNote> InternalNotes { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Organization>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.Slug).IsUnique();
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.UpdateAt).HasDefaultValueSql("NOW()");
    });

    // User
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.Email).IsUnique();
      entity.HasIndex(e => e.OrganizationId);
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      entity.HasOne(e => e.Organization)
              .WithMany(o => o.Users)
              .HasForeignKey(e => e.OrganizationId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // RefreshToken
    modelBuilder.Entity<RefreshToken>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.UserId);
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.HasOne(e => e.User)
              .WithMany(u => u.RefreshTokens)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // ← AJOUTE ICI : Property
    modelBuilder.Entity<Property>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.OrganizationId);
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => e.City);
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.Price).HasPrecision(18, 2);
      entity.Property(e => e.Surface).HasPrecision(10, 2);

      entity.HasOne(e => e.Organization)
              .WithMany()
              .HasForeignKey(e => e.OrganizationId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // Lead
    modelBuilder.Entity<Lead>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.OrganizationId);
      entity.HasIndex(e => e.PropertyId);
      entity.HasIndex(e => e.Status);
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      entity.HasOne(e => e.Organization)
              .WithMany()
              .HasForeignKey(e => e.OrganizationId)
              .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Property)
              .WithMany(p => p.Leads)
              .HasForeignKey(e => e.PropertyId)
              .OnDelete(DeleteBehavior.SetNull);
    });

    // Visit
    modelBuilder.Entity<Visit>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.PropertyId);
      entity.HasIndex(e => e.OrganizationId);
      entity.HasIndex(e => e.ScheduledAt);
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      entity.HasOne(e => e.Property)
              .WithMany(p => p.Visits)
              .HasForeignKey(e => e.PropertyId)
              .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.Organization)
              .WithMany()
              .HasForeignKey(e => e.OrganizationId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // InternalNote
    modelBuilder.Entity<InternalNote>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.OrganizationId);
      entity.HasIndex(e => e.EntityType);
      entity.HasIndex(e => e.EntityId);
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      entity.HasOne(e => e.Organization)
              .WithMany()
              .HasForeignKey(e => e.OrganizationId)
              .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }
}