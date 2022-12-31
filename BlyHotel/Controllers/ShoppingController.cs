using BlyHotel.Code;
using BlyHotel.Models;
using Microsoft.Win32;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BlyHotel.Controllers
{
    public class ShoppingController : Controller
    {
		QLKSEntities2 db = new QLKSEntities2();
        // GET: Shopping
        public ActionResult Index(string searchString,int? page)
        {
			//var dsphong = ProductsPaged(page);
			//return View(dsphong);
			if (searchString == null)
			{
				var dsphong = ProductsPaged(page);
				return View(dsphong);

			}
			else
			{ 
				if (page == null) page = 1;
				var s = searchString.ToLower();
				var result = db.LOAI_PHONG
								.Where(b => b.LP_TENLOAI.ToLower().Contains(s))
								.OrderBy(x => x.LP_IDLOAI)
								.ToList();
				int PageSize = 8;
				int PageNumber = (page ?? 1);
				return View(result.ToPagedList(PageNumber, PageSize));
			}
		}
		public IEnumerable<LOAI_PHONG> ProductsPaged(int? page)
		{
			int PageSize = 6;
			int PageNumber = (page ?? 1);
			return db.LOAI_PHONG.OrderBy(x => x.LP_IDLOAI).ToPagedList(PageNumber, PageSize);
		}


		public ActionResult DetailRoom(int? id)
		{

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
		[HttpPost]
		public JsonResult AddToCart(int id)
		{
			List<BookRoomModel> room;
			if (Session["ShoppingCart"] == null)
			{
				room = new List<BookRoomModel>();
				room.Add(new BookRoomModel { soluong = 1, loaiphong = db.LOAI_PHONG.Find(id)});
				Session["ShoppingCart"] = room;
			}
			else
			{
				bool flag = false;
				room = (List<BookRoomModel>)Session["ShoppingCart"];
				foreach (BookRoomModel item in room)
				{
					if (item.loaiphong.LP_IDLOAI == id)
					{
						item.soluong++; flag = true;
						break;
					}
				}

				if (!flag)
					room.Add(new BookRoomModel { soluong = 1, loaiphong = db.LOAI_PHONG.Find(id) });
				Session["ShoppingCart"] = room;
			}
			int roomcount = 0;
			List<BookRoomModel> ls = (List<BookRoomModel>)Session["ShoppingCart"];
			foreach (BookRoomModel item in ls)
			{
				roomcount += item.soluong;
			}
			return Json(new { ItemAmount = roomcount });

		}
		[HttpPost]
		public JsonResult Update(int id, string songayo)
		{
			List<BookRoomModel> listCartItem = (List<BookRoomModel>)Session["ShoppingCart"];
			string err = "";
			double sum = 0, total = 0;
			int cartcount = 0;
			if (!int.TryParse(songayo, out int a) || int.Parse(songayo) < 0)
			{
				err = "Số lượng không hợp lệ";
			}
			else
			{
				if (Session["ShoppingCart"] != null)
				{
					foreach (BookRoomModel item in listCartItem)
					{
						if (item.loaiphong.LP_IDLOAI == id)
						{
							item.tongngay = a;
							sum = item.SumPrice();
							break;
						}
					}
					Session["ShoppingCart"] = listCartItem;
				}
			}
			foreach (BookRoomModel i in listCartItem)
			{
				total += i.SumPrice();
				cartcount += i.soluong;
			}

			return Json(new { ItemAmount = a, SumPrice = sum, Total = total, Error = err, cartcount });
		}


		[HttpPost]
		public RedirectToRouteResult RemoveCart(int RoomID)
		{
			List<BookRoomModel> cart = (List<BookRoomModel>)Session["ShoppingCart"];
			BookRoomModel remove = cart.SingleOrDefault(m => m.loaiphong.LP_IDLOAI == RoomID);
			if (remove != null)
			{
				cart.Remove(remove);
			}

			return RedirectToAction("BookRoom");
		}

		public ActionResult BookRoom()
		{
			return View();
		}
		[HttpPost]
		public ActionResult Buy(CHI_TIET_DAT chitietdat, double Total)
		{
			if (SessionHelper.GetSession() == null) return RedirectToAction("ShoppingCart");
			List<BookRoomModel> room = (List<BookRoomModel>)Session["ShoppingCart"];
			var user = SessionHelper.GetSession().Get();
			NGUOI_DUNG account = db.NGUOI_DUNG.Where(a => a.ND_TAIKHOAN == user).SingleOrDefault();

			CreateReceipt(account, chitietdat, room, Total);

			return Redirect("/Shopping/getReserveTicket");
		}

		public void CreateReceipt(NGUOI_DUNG account, CHI_TIET_DAT ctd, List<BookRoomModel> room, double Total)
		{
			PHIEU_DAT phieudat = new PHIEU_DAT()
			{
				ND_IDNGUOIDUNG = account.ND_IDNGUOIDUNG,
				PD_NGAYDAT = DateTime.Now,
				PD_TONGTIEN = (decimal?)Total
			};
			try
			{
				db.PHIEU_DAT.Add(phieudat);
				db.SaveChanges();
			}
			catch (RetryLimitExceededException)
			{
				ModelState.AddModelError("", "Error Save Data");
			}
			foreach (BookRoomModel item in room)
			{
				CHI_TIET_DAT chitietdat = new CHI_TIET_DAT()
				{
					CTD_NGAYDEN = ctd.CTD_NGAYDEN,
					CTD_SONGAYO = item.tongngay,
					LP_IDLOAI = item.loaiphong.LP_IDLOAI,
					PD_IDPHIEU = phieudat.PD_IDPHIEU,
					CTD_SOLUONGPHONG = item.soluong
				};
				db.CHI_TIET_DAT.Add(chitietdat);
				db.SaveChanges();
			}
			Session.Remove("ShoppingCart");
		}


		public ActionResult getReserveTicket(int? page)
		{
			if (SessionHelper.GetSession() == null) return RedirectToAction("Index");
			if (page == null) page = 1;
			int PageSize = 6;
			int PageNumber = (page ?? 1);
			var user = SessionHelper.GetSession().Get();
			NGUOI_DUNG nguoidung = db.NGUOI_DUNG.Where(a => a.ND_TAIKHOAN == user).SingleOrDefault();

			var list = db.PHIEU_DAT.Where(s => s.ND_IDNGUOIDUNG == nguoidung.ND_IDNGUOIDUNG).OrderBy(x => x.PD_IDPHIEU).ToPagedList(PageNumber, PageSize);
			return View(list);
		}

		public ActionResult getDetailTicket(int? id, int? page)
		{
			if (SessionHelper.GetSession() == null) return RedirectToAction("Index");
			if (page == null) page = 1;
			int PageSize = 4;
			int PageNumber = (page ?? 1);
			var list = db.CHI_TIET_DAT.Where(s => s.PD_IDPHIEU == id).OrderBy(x => x.PD_IDPHIEU).ToPagedList(PageNumber, PageSize);

			return View(list);
		}
	}
}