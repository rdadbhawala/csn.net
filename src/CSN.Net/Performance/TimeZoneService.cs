using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Performance
{
    class TimeZoneService
    {
		public static readonly TimeZoneService S = new TimeZoneService();

		private TimeZoneService()
		{ }

		public CsnTimeZones GetTimeZones()
		{
			CsnTimeZones tzs = new CsnTimeZones();
			tzs.AllTimeZones = TimeZoneInfo.GetSystemTimeZones().Select(x => new CsnTimeZone(x)).ToArray();
			List<CsnTimeZone> newList = new List<CsnTimeZone>();
			for (int i = 0; i < 1; i++)
			{
				newList.AddRange(tzs.AllTimeZones);
			}
			tzs.AllTimeZones = newList.ToArray();
			return tzs;
		}
    }
}
