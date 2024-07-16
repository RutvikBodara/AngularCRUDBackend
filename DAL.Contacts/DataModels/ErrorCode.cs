using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.DataModels;

public partial class ErrorCode
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ErrorCode")]
    public int ErrorCode1 { get; set; }

    [Column(TypeName = "character varying")]
    public string ErrorDescription { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string Segment { get; set; } = null!;
}
