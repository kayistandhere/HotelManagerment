using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Hotel.Models;

[Table("NhanVien")]
[Index("MaNv", Name = "NhanVien_maNV", IsUnique = true)]
[Index("CCccd", Name = "UQ__NhanVien__8BCCF4CB29A996BC", IsUnique = true)]
public partial class NhanVienModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maNV")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNv { get; set; } = null!;

    [Column("tenNV")]
    [StringLength(50)]
    public string TenNv { get; set; } = null!;

    [Column("hinhAnh", TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    [Column("soDT")]
    [StringLength(11)]
    [Unicode(false)]
    public string SoDt { get; set; } = null!;

    [Column("cCCCD")]
    [StringLength(12)]
    [Unicode(false)]
    public string CCccd { get; set; } = null!;

    [Column("diaChi")]
    [StringLength(255)]
    public string? DiaChi { get; set; }

    [Column("email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("userName")]
    [StringLength(225)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    public virtual ICollection<HoaDonDatPhongModel> HoaDonDatPhongs { get; } = new List<HoaDonDatPhongModel>();

    public virtual ICollection<LuongNhanVienModel> LuongNhanViens { get; } = new List<LuongNhanVienModel>();

    public virtual TaiKhoanModel? UserNameNavigation { get; set; } = null!;

    [NotMapped]
    [DataType(DataType.Upload)]
    // [FileExtensions(Extensions = "jpg, png, gif, jpeg")]
    public IFormFile? fileUpload {get;set;}
}
