using Microsoft.EntityFrameworkCore;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Services;

namespace TerraPulse.Api.Data;

public class TerraPulseDbContext : DbContext
{
    public TerraPulseDbContext(DbContextOptions<TerraPulseDbContext> options)
        : base(options)
    {
    }

    public DbSet<AreaMonitorada> AreasMonitoradas { get; set; }
    public DbSet<IndicadorAmbiental> IndicadoresAmbientais { get; set; }
    public DbSet<AlertaAmbiental> AlertasAmbientais { get; set; }
    public DbSet<RecomendacaoSustentavel> RecomendacoesSustentaveis { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AreaMonitorada>(entity =>
        {
            entity.ToTable("AreasMonitoradas");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.Pais)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(x => x.Regiao)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.Cultura)
                .HasMaxLength(100);

            entity.Property(x => x.TamanhoHectares)
                .HasPrecision(18, 2);

            entity.Property(x => x.Latitude)
                .HasPrecision(10, 6);

            entity.Property(x => x.Longitude)
                .HasPrecision(10, 6);

            entity.HasMany(x => x.Indicadores)
                .WithOne(x => x.AreaMonitorada)
                .HasForeignKey(x => x.AreaMonitoradaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Alertas)
                .WithOne(x => x.AreaMonitorada)
                .HasForeignKey(x => x.AreaMonitoradaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IndicadorAmbiental>(entity =>
        {
            entity.ToTable("IndicadoresAmbientais");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Valor)
                .HasPrecision(18, 4);

            entity.Property(x => x.UnidadeMedida)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.Fonte)
                .HasMaxLength(150);

            entity.Property(x => x.Observacao)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<AlertaAmbiental>(entity =>
        {
            entity.ToTable("AlertasAmbientais");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(160);

            entity.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(x => x.NivelConfianca)
                .HasPrecision(5, 2);

            entity.HasMany(x => x.Recomendacoes)
                .WithOne(x => x.AlertaAmbiental)
                .HasForeignKey(x => x.AlertaAmbientalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecomendacaoSustentavel>(entity =>
        {
            entity.ToTable("RecomendacoesSustentaveis");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(160);

            entity.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(x => x.BeneficioEsperado)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(160);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.SenhaHash)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Usuario>().HasData(new Usuario
        {
            Id = 1,
            Nome = "Administrador TerraPulse",
            Email = "admin@terrapulse.ai",
            SenhaHash = PasswordHasher.ComputeHash("Terra@123"),
            Perfil = PerfilUsuario.Administrador,
            Ativo = true,
            DataCadastro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}