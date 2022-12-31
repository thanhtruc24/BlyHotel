using BlyHotel.Code;
using BlyHotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlyHotel.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(LoginModel model)
		{
			var result = new AccountModel().Login(model.Taikhoan, model.Matkhau);
			var permission = new AccountModel().GetPermission(model.Taikhoan);
			if (permission)
			{
				if (result && ModelState.IsValid)
				{
					//Nếu thành công chúng ta cần tạo session
					SessionHelper.SetSession(new UserSession(model.Taikhoan));
					SessionHelper.SetAdmin();
					return Redirect("/Admin/Room/getListRoom");
				}
				else
				{ ViewBag.error = "Tài khoản hoặc mặt khẩu không chính xác!"; }
			}
			else { ViewBag.error = "Tài khoản không hợp lệ"; }
			return View(model);
		}


	}
}