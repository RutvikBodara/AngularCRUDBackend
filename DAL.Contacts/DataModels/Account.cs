using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contacts.DataModels;

public partial class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username", TypeName = "character varying")]
    public string Username { get; set; } = null!;

    [Column("password", TypeName = "character varying")]
    public string Password { get; set; } = null!;

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

    [Column("mobilenumber")]
    [Precision(10, 0)]
    public decimal? Mobilenumber { get; set; }

    [Column("emailid", TypeName = "character varying")]
    public string? Emailid { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("accounttype")]
    public int Accounttype { get; set; }

    [Column("firstname", TypeName = "character varying")]
    public string Firstname { get; set; } = null!;

    [Column("lastname", TypeName = "character varying")]
    public string Lastname { get; set; } = null!;
}
