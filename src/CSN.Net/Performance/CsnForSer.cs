using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using Csn;

namespace Performance
{
	class CsnForSer
		: ISerializer
	{
		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			Writer w = new Writer(sw);
			// typedefs
			long ttType = w.WriteTypeDef("TransitionTime", "IsFixedDateRule", "Day", "Month", "TimeOfDay", "Week", "DayOfWeek").Current;
			long adjType = w.WriteTypeDef("Adjustment", "StartDate", "EndDate", "DaylightDeltaHours", "TransitionStart", "TransitionEnd").Current;
			long tzType = w.WriteTypeDef("TimeZone", "Id", "DisplayName", "DaylightName", "StandardName", "HasDst", "UtcOffsetHours", "Adjustments").Current;
			long tzsType = w.WriteTypeDef("TimeZones", "AllTimeZones").Current;

			var rcTzs = ctzs.AllTimeZones.Select(x => WriteTz(x, w, tzType, adjType, ttType));
			long rc = w.WriteArray().WRef(rcTzs.ToArray()).Current;
			w.WriteInstance(tzsType).WRef(rc);
			sw.Flush();
		}

		private long? WriteTz(CsnTimeZone x, Writer w, long tzType, long adjType, long ttType)
		{
			long? rcAdjArr = null;
			if (x.Adjustments != null)
			{
				var rcAdjs = x.Adjustments.Select(y => WriteAdj(y, w, adjType, ttType));
				rcAdjArr = w.WriteArray().WRef(rcAdjs.ToArray()).Current;
			}
			return w.WriteInstance(tzType).W(x.Id).W(x.DisplayName).W(x.DaylightName).W(x.StandardName).W(x.HasDst).W(x.UtcOffsetHours).WRef(rcAdjArr).Current;
		}

		private long? WriteAdj(CsnAdjustment y,  Writer w, long adjType, long ttType)
		{
			long rc1 = WriteTrTime(y.TransitionStart, w, ttType);
			long rc2 = WriteTrTime(y.TransitionEnd, w, ttType);
			return w.WriteInstance(adjType).W(y.StartDate).W(y.EndDate).W(y.DaylightDeltaHours).WRef(rc1).WRef(rc2).Current;
		}

		private long WriteTrTime(CsnTransition tt, Writer w, long ttType)
		{
			return w.WriteInstance(ttType).W(tt.IsFixedDateRule).W(tt.Day).W(tt.Month).W(tt.TimeOfDay).W(tt.Week).W(tt.DayOfWeek).Current;
		}

		public CsnTimeZones Deserialize(StreamReader sw)
		{
			throw new NotImplementedException();
		}
	}
}
