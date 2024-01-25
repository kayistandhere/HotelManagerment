using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Index("IdRole", Name = "Roles_idRole", IsUnique = true)]
public partial class RoleModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idRole")]
    public int IdRole { get; set; }

    [Column("roleName")]
    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<TaiKhoanModel> TaiKhoans { get; } = new List<TaiKhoanModel>();
}
