using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;

namespace ApiFilm.Models.EntityFramework;

[Table("t_e_film_flm")]
public partial class Film
{
    [Key]
    [Column("flm_id")]
    public int FilmId { get; set; }

    [Column("flm_titre")]
    [StringLength(100)]
    public string Titre { get; set; } = null!;

    [Column("flm_resume")]
    public string? Resume { get; set; }

    [Column("flm_datesortie")]
    public DateTime DateSortie { get; set; }

    [Column("flm_duree", TypeName = "numeric(3,0)")]
    public decimal Duree { get; set; }

    [Column("flm_genre")]
    [StringLength(30)]
    public string Genre { get; set; } = null!;

    [InverseProperty("FilmNote")]
    public virtual ICollection<Notation> NotesFilm { get; set; } = new List<Notation>();
}