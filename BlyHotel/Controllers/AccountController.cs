using BlyHotel.Code;
using BlyHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace BlyHotel.Controllers
{
    public class AccountController : Controller
    {
		private QLKSEntities2 db = new QLKSEntities2();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
			var result = new AccountModel().Login(model.Taikhoan, model.Matkhau);
			if (result && ModelState.IsValid)
			{
				//Nếu thành công chúng ta cần tạo session
				SessionHelper.SetSession(new UserSession(model.Taikhoan));
				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.error = "Tài khoản hoặc mặt khẩu không chính xác!";
			}
			return View(model);

		}


		public ActionResult Logout()
		{
			SessionHelper.RemoveSession();
			return Redirect("/");
		}

		public ActionResult Register()
		{
			if (SessionHelper.GetSession() != null)
			{ return Redirect("/"); }
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel model)
		{
			var res = db.NGUOI_DUNG.Where(a => a.ND_TAIKHOAN == model.Taikhoan.ToString()).SingleOrDefault();
			if (res != null)
			{
				ViewBag.error = "Tài khoản đã tồn tại!";
			}
			else
			{
				if (model.Matkhau == model.MatKhauNhapLai)
				{
					NGUOI_DUNG account = new NGUOI_DUNG { ND_HOTEN = model.Hoten, ND_TAIKHOAN = model.Taikhoan, ND_MATKHAU = model.Matkhau, ND_QUYENSD = 0 };
					try
					{
						if (ModelState.IsValid)
						{
							db.NGUOI_DUNG.Add(account);
							db.SaveChanges();
							//ViewBag.success = "Đăng ký thành công";
							//ViewBag.job = "Đăng ký tài khoản";
							return Redirect("/Account/Index");
						}
					}
					catch (RetryLimitExceededException)
					{
						ModelState.AddModelError("", "Error Save Data");
					}
				}
				else
				{
					ViewBag.error = "Mật khẩu nhập lại không đúng!";
				}
			}
			return View(model);
		}

		public ActionResult CustomerProfile()
		{
			if (SessionHelper.GetSession() == null)
			{ return Redirect("/Home/Index"); }

			var taikhoan = SessionHelper.GetSession().Get();
			var profile = db.NGUOI_DUNG.Where(s => s.ND_TAIKHOAN == taikhoan).SingleOrDefault();

			return View(profile);
		}
		[HttpGet]
		public ActionResult EditProfile(int? id)
		{
			if (SessionHelper.GetSession() == null)
			{ return Redirect("/"); }
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			NGUOI_DUNG nguoidung = db.NGUOI_DUNG.SingleOrDefault(s => s.ND_IDNGUOIDUNG== id);
			return View(nguoidung);

		}
		[HttpPost, ActionName("EditProfile")]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult EditProfile(int id)
		{ 
			var nguoidung = db.NGUOI_DUNG.Find(id);
			if (TryUpdateModel(nguoidung, "", new string[] { "ND_HOTEN", "ND_EMAIL", "ND_TAIKHOAN", "ND_AVT" }))
			{
				try
				{
					db.Entry(nguoidung).State = EntityState.Modified;
					db.SaveChanges();
				}
				catch (RetryLimitExceededException)
				{
					ModelState.AddModelError("", "Error Save Data");
				}
				return RedirectToAction("CustomerProfile");
			}

			return View(nguoidung);
		}


	}
}