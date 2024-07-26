using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.DataModels;

[Table("Category")]
public partial class Category
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("isDeleted")]
    public bool IsDeleted { get; set; }

    [Column("updatedby", TypeName = "timestamp without time zone")]
    public DateTime? Updatedby { get; set; }

    [Column("deletedby", TypeName = "timestamp without time zone")]
    public DateTime? Deletedby { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
