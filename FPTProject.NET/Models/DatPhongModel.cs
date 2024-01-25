using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("DatPhong")]
[Index("MaDp", Name = "DatPhong_maDP", IsUnique = true)]
public partial class DatPhongModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maDP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDp { get; set; } = null!;

    [Column("soNguoi")]
    [Required(ErrorMessage = "Số Người không được để trống!")]
    public int? SoNguoi { get; set; }

    [Column("ngayBatDau", TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column("ngayKetThuc", TypeName = "datetime")]
    public DateTime NgayKetThuc { get; set; }

    [Column("maKH")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaKh { get; set; } = null!;

    [Column("maP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaP { get; set; } = null!;

    [Column("tongTien", TypeName = "decimal(12, 0)")]
    public decimal? TongTien { get; set; }

    public virtual ICollection<DatDichVuModel> DatDichVus { get; } = new List<DatDichVuModel>();

    public virtual HoaDonDatPhongModel? HoaDonDatPhong { get; set; }

    public virtual KhachHangModel? MaKhNavigation { get; set; } = null!;

    public virtual PhongModel? MaPNavigation { get; set; } = null!;
}
