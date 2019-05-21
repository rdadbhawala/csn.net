using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using Abstraction.Csn;

namespace Performance
{
	class CsnSer
		: ISerializer
	{
		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			Writer w = new Writer(sw, Config.CreateDefaultConfig());
			// typedefs
			RecordCode ttType = w.WriteTypeDefRecord("TransitionTime", "IsFixedDateRule", "Day", "Month", "TimeOfDay", "Week", "DayOfWeek");
			RecordCode adjType = w.WriteTypeDefRecord("Adjustment", "StartDate", "EndDate", "DaylightDeltaHours", "TransitionStart", "TransitionEnd");
			RecordCode tzType = w.WriteTypeDefRecord("TimeZone", "Id", "DisplayName", "DaylightName", "StandardName", "HasDst", "UtcOffsetHours", "Adjustments");
			RecordCode tzsType = w.WriteTypeDefRecord("TimeZones", "AllTimeZones");

			int tzLen = ctzs.AllTimeZones.Length;
			RecordCode[] rcTzsArr = new RecordCode[tzLen];
			for (int tzCtr = 0; tzCtr < tzLen; tzCtr++)
			{
				CsnTimeZone ctz = ctzs.AllTimeZones[tzCtr];
				if (ctz.Adjustments != null)
				{
					int adjLen = ctz.Adjustments.Length;
					RecordCode rcAdjArr = null;
					if (adjLen > 0)
					{
						RecordCode[] arrAdjs = new RecordCode[adjLen];
						RecordCode outRcAdj = null;
						for (int adjCtr = 0; adjCtr < adjLen; adjCtr++)
						{
							CsnAdjustment adj = ctz.Adjustments[adjCtr];
							arrAdjs[adjCtr] = w.WriteInstanceRecord(adjType, adj.StartDate, adj.EndDate, adj.DaylightDeltaHours,
								WriteTrTime(adj.TransitionStart, w, ttType), WriteTrTime(adj.TransitionEnd, w, ttType));
						}
						rcAdjArr = w.WriteArrayRecord(adjType, arrAdjs);
					}
					rcTzsArr[tzCtr] = w.WriteInstanceRecord(tzType, ctz.Id, ctz.DisplayName, ctz.DaylightName, ctz.StandardName, ctz.HasDst, ctz.UtcOffsetHours, rcAdjArr);
				}
			}
			w.WriteInstanceRecord(tzsType, w.WriteArrayRecord(tzType, rcTzsArr));
			sw.Flush();
		}

		private CastPrimitive WriteTrTime(CsnTransition tt, Writer w, RecordCode ttType)
		{
			return (tt == null) ?
				null :
				w.WriteInstanceFields(ttType).W(tt.IsFixedDateRule).W(tt.Day).W(tt.Month).W(tt.TimeOfDay).W(tt.Week).W(tt.DayOfWeek).Current;

			//return (tt == null) ?
			//	null :
			//	w.WriteInstanceRecord(ttType, tt.IsFixedDateRule, tt.Day, tt.Month, tt.TimeOfDay, tt.Week, tt.DayOfWeek);
		}
	}
}
