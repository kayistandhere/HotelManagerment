using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("HoaDonDatPhong")]
[Index("MaHddp", Name = "HoaDonDatPhong_maHDDP", IsUnique = true)]
[Index("MaDp", Name = "UQ__HoaDonDa__7A3EF4169A5B4E3A", IsUnique = true)]
public partial class HoaDonDatPhongModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maHDDP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaHddp { get; set; } = null!;

    [Column("ngayHD", TypeName = "datetime")]
    public DateTime NgayHd { get; set; }

    [Column("maNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNv { get; set; } = null!;

    [Column("maDP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDp { get; set; } = null!;

    public virtual DatPhongModel? MaDpNavigation { get; set; } = null!;

    public virtual NhanVienModel? MaNvNavigation { get; set; } = null!;
}
