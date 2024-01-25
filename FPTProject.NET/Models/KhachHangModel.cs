using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("KhachHang")]
[Index("MaKh", Name = "KhachHang_maKH", IsUnique = true)]
[Index("CCcd", Name = "UQ__KhachHan__201429D006918EF3", IsUnique = true)]
public partial class KhachHangModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maKH")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaKh { get; set; } = null!;

    [Column("tenKH")]
    [StringLength(50)]
    public string TenKh { get; set; } = null!;

    [Column("hinhAnh", TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    [Column("soDT")]
    [StringLength(11)]
    [Unicode(false)]
    [Required]
    public string? SoDt { get; set; }

    [Column("cCCD")]
    [StringLength(12)]
    [Unicode(false)]
    public string CCcd { get; set; } = null!;

    [Column("diaChi")]
    [StringLength(255)]
    public string? DiaChi { get; set; }

    public virtual ICollection<DatPhongModel> DatPhongs { get; } = new List<DatPhongModel>();
}
