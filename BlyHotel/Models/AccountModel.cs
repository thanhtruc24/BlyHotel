using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using PagedList;

namespace BlyHotel.Models
{
	public class AccountModel
	{
		private QLKSEntities2 context = null;
		public AccountModel()
		{
			context = new QLKSEntities2();
		}

		public bool GetPermission(string email)
		{
			var sql = from s in context.NGUOI_DUNG where s.ND_TAIKHOAN == email select s.ND_QUYENSD;
			int? res = sql.FirstOrDefault();
			if (res == 1) { return true; }
			else { return false; }
		}

		public bool Login(string Taikhoan, string Password)
		{
			object[] sqlParas =
			{
				new SqlParameter("@ND_TAIKHOAN", Taikhoan),
				new SqlParameter("ND_MATKHAU", Password),
			};
			var res = context.Database.SqlQuery<bool>("Sp_Account_Login @ND_TAIKHOAN, @ND_MATKHAU", sqlParas).SingleOrDefault();
			return	res;

		}

		
	}
}