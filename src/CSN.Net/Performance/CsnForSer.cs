using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using Abstraction.Csn;

namespace Performance
{
	class CsnForSer
		: ISerializer
	{
		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			Writer w = new Writer(sw);
			// typedefs
			RecordCode ttType = w.WriteTypeDefRecord("TransitionTime", "IsFixedDateRule", "Day", "Month", "TimeOfDay", "Week", "DayOfWeek").Current;
			RecordCode adjType = w.WriteTypeDefRecord("Adjustment", "StartDate", "EndDate", "DaylightDeltaHours", "TransitionStart", "TransitionEnd").Current;
			RecordCode tzType = w.WriteTypeDefRecord("TimeZone", "Id", "DisplayName", "DaylightName", "StandardName", "HasDst", "UtcOffsetHours", "Adjustments").Current;
			RecordCode tzsType = w.WriteTypeDefRecord("TimeZones", "AllTimeZones").Current;

			var rcTzs = ctzs.AllTimeZones.Select(x => WriteTz(x, w, tzType, adjType, ttType));
			RecordCode rc = w.WriteArrayRecord(tzType, rcTzs.ToArray()).Current;
			w.WriteInstanceFields(tzsType).W(rc);
			sw.Flush();
		}

		private RecordCode WriteTz(CsnTimeZone x, Writer w, RecordCode tzType, RecordCode adjType, RecordCode ttType)
		{
			RecordCode rcAdjArr = null;
			if (x.Adjustments != null)
			{
				var rcAdjs = x.Adjustments.Select(y => WriteAdj(y, w, adjType, ttType));
				rcAdjArr = w.WriteArrayRecord(adjType, rcAdjs.ToArray()).Current;
			}
			return w.WriteInstanceFields(tzType).W(x.Id).W(x.DisplayName).W(x.DaylightName).W(x.StandardName).W(x.HasDst).W(x.UtcOffsetHours).W(rcAdjArr).Current;
		}

		private RecordCode WriteAdj(CsnAdjustment y,  Writer w, RecordCode adjType, RecordCode ttType)
		{
			RecordCode rc1 = WriteTrTime(y.TransitionStart, w, ttType);
			RecordCode rc2 = WriteTrTime(y.TransitionEnd, w, ttType);
			return w.WriteInstanceFields(adjType).W(y.StartDate).W(y.EndDate).W(y.DaylightDeltaHours).W(rc1).W(rc2).Current;
		}

		private RecordCode WriteTrTime(CsnTransition tt, Writer w, RecordCode ttType)
		{
			return (tt == null) ?
				null :
				w.WriteInstanceFields(ttType).W(tt.IsFixedDateRule).W(tt.Day).W(tt.Month).W(tt.TimeOfDay).W(tt.Week).W(tt.DayOfWeek).Current;
		}

		public CsnTimeZones Deserialize(StreamReader sw)
		{
			throw new NotImplementedException();
		}
	}
}
