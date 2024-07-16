using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.DataModels;

[Index("ContactTypeId", Name = "fki_contact_type_fkey")]
[Index("ContactTypeId", Name = "fki_dsaf")]
public partial class Contact
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("surname", TypeName = "character varying")]
    public string Surname { get; set; } = null!;

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [Column("contactTypeId")]
    public int? ContactTypeId { get; set; }

    [ForeignKey("ContactTypeId")]
    [InverseProperty("Contacts")]
    public virtual ContactType? ContactType { get; set; }
}
