using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlyHotel.Code
{
	[Serializable]
	public class UserSession
	{
		private string Username;


		public string Get()
		{
			return Username;
		}

		private void Set(string value)
		{
			Username = value;
		}
		public UserSession(string Email)
		{
			this.Username = Email;
		}
	}
}