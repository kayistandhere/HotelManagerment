using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("TacVuPhong")]
[Index("MaTvp", Name = "TacVuPhong_maTVP", IsUnique = true)]
public partial class TacVuPhongModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maTVP")]
    public int MaTvp { get; set; }

    [Column("tenTVP")]
    [StringLength(50)]
    public string? TenTvp { get; set; }

    public virtual ICollection<PhongModel> Phongs { get; } = new List<PhongModel>();
}
