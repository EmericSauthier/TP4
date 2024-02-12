using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiFilm.Models.EntityFramework;

public partial class FilmsDBContext : DbContext
{
    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

    public FilmsDBContext()
    {
    }

    public FilmsDBContext(DbContextOptions<FilmsDBContext> options)
    : base(options)
    {
    }

    public virtual DbSet<Film> Films { get; set; }

    public virtual DbSet<Notation> Notations { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseLoggerFactory(MyLoggerFactory)
                          .EnableSensitiveDataLogging()
                          .UseNpgsql("Server=localhost;port=5432;Database=FilmsDB;uid=postgres;password=postgres;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Film>(entity =>
        {
            entity.HasKey(e => e.FilmId).HasName("pk_film");
        });

        modelBuilder.Entity<Notation>(entity =>
        {
            entity.HasKey(e => new { e.UtilisateurId, e.FilmId }).HasName("pk_notation");

            entity.HasOne(d => d.FilmNote).WithMany(p => p.NotesFilm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notation_film");

            entity.HasOne(d => d.UtilisateurNotant).WithMany(p => p.NotesUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notation_utilisateur");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurId).HasName("pk_utilisateur");

            entity.HasIndex(e => e.Mail).IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
