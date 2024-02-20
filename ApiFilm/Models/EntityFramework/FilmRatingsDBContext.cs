using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiFilm.Models.EntityFramework;

public partial class FilmRatingsDBContext : DbContext
{
    public FilmRatingsDBContext() { }
    public FilmRatingsDBContext(DbContextOptions<FilmRatingsDBContext> options) : base(options) { }

    public virtual DbSet<Film> Films { get; set; } = null!;
    public virtual DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
    public virtual DbSet<Notation> Notations { get; set; } = null!;

    //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        {
    //            if (!optionsBuilder.IsConfigured)
    //            {
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //                optionsBuilder.UseNpgsql("Server=localhost;port=5432;Database=FilmRatingsDB; uid=postgres; password=postgres;");
    //            }
    //        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Film>(entity =>
        {
            entity.HasKey(e => e.FilmId)
                .HasName("pk_flm");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurId)
                .HasName("pk_utl");

            entity.Property(e => e.CodePostal).IsFixedLength();

            entity.Property(e => e.Pays).HasDefaultValue("France");

            entity.Property(e => e.DateCreation).HasDefaultValueSql("now()");

            entity.Property(e => e.Mobile).IsFixedLength();
        });

        modelBuilder.Entity<Notation>(entity =>
        {
            entity.HasKey(e => new { e.UtilisateurId, e.FilmId })
                .HasName("pk_not");

            entity.HasCheckConstraint("ck_not_note", "not_note between 0 and 5");


            entity.HasOne(d => d.FilmNote)
                .WithMany(p => p.NotesFilm)
                .HasForeignKey(d => d.FilmId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_not_flm");

            entity.HasOne(d => d.UtilisateurNotant)
                .WithMany(p => p.NotesUtilisateur)
                .HasForeignKey(d => d.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_not_utl");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
