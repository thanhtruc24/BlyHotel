using BlyHotel.Code;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BlyHotel.Areas.Admin.Controllers
{
	public class CustomerController : Controller
	{
		QLKSEntities2 db = new QLKSEntities2();
		// GET: Admin/Customer
		public ActionResult Index(int? page)
		{
			if (!SessionHelper.IsAdmin()) return Redirect("/Admin/Home/Index");
			if (page == null) page = 1;
			int PageSize = 8;
			int PageNumber = (page ?? 1);
			var list = (from s in db.NGUOI_DUNG select s).Where(s => s.ND_QUYENSD != 1).OrderBy(x => x.ND_IDNGUOIDUNG).ToPagedList(PageNumber, PageSize);
			return View(list);

		}

		public ActionResult Delete(int id)
		{
			NGUOI_DUNG nguoidung = db.NGUOI_DUNG.SingleOrDefault(n => n.ND_IDNGUOIDUNG == id);
			if (nguoidung == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(nguoidung);
		}

		//Xác nhận xoá
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirm(int id)
		{
			NGUOI_DUNG nguoidung = db.NGUOI_DUNG.SingleOrDefault(n => n.ND_IDNGUOIDUNG == id);
			if (nguoidung == null)
			{ 
				Response.StatusCode = 404;
				return null;
			}
			db.NGUOI_DUNG.Remove(nguoidung);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		//xem danh sách phiếu đặt
		public ActionResult getReserveTicket(int? page, int? id)
		{
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");
			if (page == null) page = 1;
			int PageSize = 6;
			int PageNumber = (page ?? 1);
			var list = db.PHIEU_DAT.Where(s => s.ND_IDNGUOIDUNG == id).OrderBy(x => x.ND_IDNGUOIDUNG).ToPagedList(PageNumber, PageSize);
			return View(list);

		}

		//xem chi tiết phiếu
		public ActionResult getDetailTicket(int? page,int? id)
		{
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");
			if (page == null) page = 1;
			int PageSize = 6;
			int PageNumber = (page ?? 1);
			var list = db.CHI_TIET_DAT.Where(s => s.PD_IDPHIEU == id).OrderBy(x => x.PD_IDPHIEU).ToPagedList(PageNumber, PageSize);
			return View(list);

		}



	}
}