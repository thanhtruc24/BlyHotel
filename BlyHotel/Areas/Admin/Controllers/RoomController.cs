using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BlyHotel.Code;
using PagedList;

namespace BlyHotel.Areas.Admin.Controllers
{
    public class RoomController : Controller
    {
        // GET: Admin/Room
        QLKSEntities2 db = new QLKSEntities2();
        public ActionResult getListRoom(string searchString,int? page)
        {
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");

			//Xử lý thao tác linq đơn giản
			//var listRoom = from s in db.LOAI_PHONG select s;
			if(searchString == null)
			{
				var listRoom = ProductsPaged(page);
				return View(listRoom);
			}
			else
			{
				if (page == null) page = 1;
				var s = searchString.ToLower();
				var result = db.LOAI_PHONG
								.Where(b => b.LP_TENLOAI.ToLower().Contains(s))
								.OrderBy(x => x.LP_IDLOAI)
								.ToList();
				int PageSize = 3;
				int PageNumber = (page ?? 1);
				return View(result.ToPagedList(PageNumber, PageSize));
			}
			
		}
		//Thêm
		public ActionResult CreateRoom()
		{
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");
			return View();
		}

		[HttpPost, ActionName("CreateRoom")]
		public ActionResult CreateRoom(LOAI_PHONG loaiphong, HttpPostedFileBase fileUpload)
		{
			try
			{
				if (ModelState.IsValid)
				{
					//Upload file
					var fileName = Path.GetFileName(fileUpload.FileName);
					//Lưu đường dẫn file ảnh 
					var path = Path.Combine(Server.MapPath("~/Content/Image"), fileName);
					//Kiểm tra file đã tồn tại
					if (System.IO.File.Exists(path))
					{
						ViewBag.ThongBao = "Hình ảnh đã tồn tại";
					}
					else
					{
						fileUpload.SaveAs(path);
					}
					//Them Sach Moi
					loaiphong.LP_HINHANH = fileUpload.FileName;
					db.LOAI_PHONG.Add(loaiphong);
					db.SaveChanges();
				}
			}
			catch (RetryLimitExceededException)
			{
				ModelState.AddModelError("", "Error Save Data");
			}
			//Cập nhật lại danh sách hiển thị
			var listRoom = from s in db.LOAI_PHONG select s;
			return Redirect("/Admin/Room/getListRoom");
		}


		//Chi tiết
		public ActionResult Details(int? id)
		{
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");
			if (id == null)
			{
				return HttpNotFound();
			}
			var viewModel = db.LOAI_PHONG.SingleOrDefault(s => s.LP_IDLOAI == id);
			if (viewModel == null)
			{
				return HttpNotFound();
			}
			return View(viewModel);
		}
		//Sửa
		[HttpGet]
		public ActionResult Edit(int? id)
		{
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			LOAI_PHONG loaiphong = db.LOAI_PHONG.SingleOrDefault(s => s.LP_IDLOAI == id);
			return View(loaiphong);

		}
		[HttpPost, ActionName("Edit")]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id)
		{
			var roomUpdate = db.LOAI_PHONG.Find(id);
			if (TryUpdateModel(roomUpdate, "", new string[] { "LP_TENLOAI", "LP_GIA", "LP_SOLUONG", "LP_MOTA","LP_HINHANH", "LP_SLNguoi" }))
			{
				try
				{
					db.Entry(roomUpdate).State = EntityState.Modified;
					db.SaveChanges();
				}
				catch (RetryLimitExceededException)
				{
					ModelState.AddModelError("", "Error Save Data");
				}
			}

			return RedirectToAction("getListRoom");
		}

		[HttpGet]
		public ActionResult Delete(int id)
		{
			LOAI_PHONG loaiphong = db.LOAI_PHONG.SingleOrDefault(n => n.LP_IDLOAI == id);
			if (loaiphong == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(loaiphong);
		}

		//Xác nhận xoá
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirm(int id)
		{
			LOAI_PHONG loaiphong = db.LOAI_PHONG.SingleOrDefault(n => n.LP_IDLOAI == id);
			var path = Path.Combine(Server.MapPath("~/Content/Image"), loaiphong.LP_HINHANH );
			if (loaiphong == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			//Xoá ảnh trong thư mục ~/Content/Image
			if (System.IO.File.Exists(path))
			{
				System.IO.File.Delete(path);
			}

			db.LOAI_PHONG.Remove(loaiphong);
			db.SaveChanges();
			return RedirectToAction("getListRoom");
		}


		//Phân trang
		public IEnumerable<LOAI_PHONG> ProductsPaged(int? page)
		{
			int PageSize = 3;
			int PageNumber = (page ?? 1);
			return db.LOAI_PHONG.OrderBy(x => x.LP_IDLOAI).ToPagedList(PageNumber, PageSize);
		}
		//lấy dánh sách các phiếu đặt phòng

		public ActionResult listReserveTicket(int? page)
		{
			if (!SessionHelper.IsAdmin())
				return Redirect("/Admin/Home/Index");
			if (page == null) page = 1;
			int PageSize = 6;
			int PageNumber = (page ?? 1);
			var list = db.PHIEU_DAT.OrderBy(x => x.ND_IDNGUOIDUNG).ToPagedList(PageNumber, PageSize);
			return View(list);

		}

		//xem chi tiết phiếu
		public ActionResult getDetailTicket(int? page, int? id)
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