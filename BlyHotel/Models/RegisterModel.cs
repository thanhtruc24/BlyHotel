using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlyHotel.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "Không được để trống!")]
		[MaxLength(50, ErrorMessage = "Tên tài khoản tối đa 40 ký tự!")]
		public string Taikhoan { set; get; }
		[Required(ErrorMessage = "Không được để trống!")]
		[MaxLength(50, ErrorMessage = "Mật khẩu tối đa 50 ký tự!")]
		public string Matkhau { set; get; }
		[Required(ErrorMessage = "Không được để trống!")]
		[MaxLength(50, ErrorMessage = "Mật khẩu tối đa 50 ký tự!")]
		public string MatKhauNhapLai { get; set; }

		[Required(ErrorMessage ="Vui lòng nhập họ tên")]
		public string Hoten { get; set; }
	}
}