using Gimnasio.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Api.Data;

public class GimnasioContext(DbContextOptions<GimnasioContext> options) : DbContext(options)
{
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Membresia> Membresias => Set<Membresia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Socio>().HasIndex(s => s.Correo).IsUnique();
        modelBuilder.Entity<Membresia>().HasIndex(m => m.Codigo).IsUnique();
        modelBuilder.Entity<Membresia>()
            .HasOne(m => m.Socio)
            .WithMany(s => s.Membresias)
            .HasForeignKey(m => m.SocioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
