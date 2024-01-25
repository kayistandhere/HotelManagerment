using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("DichVu")]
[Index("MaDv", Name = "DichVu_maDV", IsUnique = true)]
public partial class DichVuModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maDV")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDv { get; set; } = null!;

    [Column("tenDV")]
    [StringLength(100)]
    public string TenDv { get; set; } = null!;

    [Column("donGia", TypeName = "decimal(19, 0)")]
    public decimal DonGia { get; set; }

    [Column("soLuong")]
    public int SoLuong { get; set; }

    [Column("hinhAnh", TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    public virtual ICollection<DatDichVuModel> DatDichVus { get; } = new List<DatDichVuModel>();
}
