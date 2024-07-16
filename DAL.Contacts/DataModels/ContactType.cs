using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.DataModels;

public partial class ContactType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name", TypeName = "character varying")]
    public string Name { get; set; } = null!;

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [InverseProperty("ContactType")]
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
