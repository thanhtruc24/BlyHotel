using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlyHotel.Code
{
	public class SessionHelper
	{
		public static void SetSession(UserSession session)
		{
			HttpContext.Current.Session["loginSession"] = session;
		}

		public static UserSession GetSession()
		{
			var session = HttpContext.Current.Session["loginSession"];
			if (session == null)
				return null;
			else
			{
				return session as UserSession;
			}
		}

		public static void RemoveSession()
		{
			HttpContext.Current.Session.Remove("loginSession");
			if (HttpContext.Current.Session["isAdmin"] != null)
			{
				HttpContext.Current.Session.Remove("isAdmin");
			}
		}
		public static bool IsAdmin()
		{
			var session = HttpContext.Current.Session["isAdmin"] ?? null;
			if (session != null)
			{
				return true;
			}
			else { return false; }
		}

		public static void SetAdmin()
		{
			HttpContext.Current.Session["isAdmin"] = true;
		}

	}
}