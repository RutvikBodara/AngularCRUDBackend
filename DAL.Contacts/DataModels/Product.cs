using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.DataModels;

[Index("Categoryid", Name = "fki_category_fkey")]
public partial class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("categoryid")]
    public int Categoryid { get; set; }

    [Column("image", TypeName = "character varying")]
    public string Image { get; set; } = null!;

    [Column("isDeleted")]
    public bool? IsDeleted { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? DeletedBy { get; set; }

    [Column("updatedby", TypeName = "timestamp without time zone")]
    public DateTime? Updatedby { get; set; }

    [Column("description", TypeName = "character varying")]
    public string Description { get; set; } = null!;

    [Column("rating")]
    public double? Rating { get; set; }

    [Column("helplineNumber")]
    [Precision(10, 0)]
    public decimal? HelplineNumber { get; set; }

    [Column("launchDate")]
    public DateOnly? LaunchDate { get; set; }

    [Column(TypeName = "character varying")]
    public string? ProductImage { get; set; }

    [Column("price")]
    public double? Price { get; set; }

    [Column("availableforsale")]
    public bool? Availableforsale { get; set; }

    [Column("countryServed")]
    public short[]? CountryServed { get; set; }

    [Column("lastDate")]
    public DateOnly? LastDate { get; set; }

    [ForeignKey("Categoryid")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;
}
