using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("Phong")]
[Index("MaP", Name = "Phong_maP", IsUnique = true)]
[Index("SoPhong", Name = "UQ__Phong__68583FADD8CC3048", IsUnique = true)]
public partial class PhongModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaP { get; set; } = null!;

    [Column("soPhong")]
    public int SoPhong { get; set; }

    [Column("hinhAnh", TypeName = "ntext")]
    public string? HinhAnh { get; set; }

    [Column("maLP")]
    public int MaLp { get; set; }

    [Column("maTVP")]
    public int MaTvp { get; set; }

    [Column("trangThaiPhong")]
    public int TrangThaiPhong { get; set; }

    [Column("moTa")]
    [StringLength(500)]
    public string? MoTa { get; set; }

    [Column("loaiGiuong")]
    public bool LoaiGiuong { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DatPhongModel> DatPhongs { get; } = new List<DatPhongModel>();

    public virtual LoaiPhongModel? MaLpNavigation { get; set; } = null!;

    public virtual TacVuPhongModel? MaTvpNavigation { get; set; } = null!;

    [NotMapped]
    [DisplayName("Upload Hình Ảnh Phòng")]
    public IFormFile? fileUpload {get;set;}
}
