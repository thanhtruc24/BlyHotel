using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlyHotel.Models
{
	public class BookRoomModel
	{
		public LOAI_PHONG loaiphong { get; set;}
		public int tongngay { get; set; }
		public int soluong { get; set; }

		public double SumPrice()
		{
			double s = 0;
			s = (double)(tongngay * loaiphong.LP_GIA * soluong);
			return s;
		}

	}
}