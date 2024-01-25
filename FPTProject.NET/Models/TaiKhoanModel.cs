using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("TaiKhoan")]
[Index("UserName", Name = "TaiKhoan_userName", IsUnique = true)]
[Index("Email", Name = "UQ__TaiKhoan__AB6E61642D7CCD9A", IsUnique = true)]
public partial class TaiKhoanModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userName")]
    [Display(Name = "Tên đăng nhập")]
    [StringLength(16, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải có ít nhất {2} ký tự và tối đa {1} ký tự!")]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [Column("userPassword")]
    [Display(Name = "Mật khẩu")]
    [StringLength(16, MinimumLength = 6, ErrorMessage = "Tên đăng nhập phải có ít nhất {2} ký tự và tối đa {1} ký tự!")]
    [Unicode(false)]
    public string UserPassword { get; set; } = null!;

    [Column("idRole")]
    public int IdRole { get; set; }

    [Column("email")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("phoneNumber")]
    [Display(Name = "Số điện thoại")]
    [StringLength(11)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [Column("emailConfirm")]
    public bool EmailConfirm { get; set; }

    [Column("twoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; }

    [Column("timeLockOut", TypeName = "datetime")]
    public DateTime? TimeLockOut { get; set; }

    [Column("coutLoginFailed")]
    public int? CoutLoginFailed { get; set; }

    public virtual RoleModel? IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<NhanVienModel> NhanViens { get; } = new List<NhanVienModel>();
}
