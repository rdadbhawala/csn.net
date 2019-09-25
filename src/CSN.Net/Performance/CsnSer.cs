using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using Csn;

namespace Performance
{
	class CsnSer
		: ISerializer
	{
		public CsnTimeZones Deserialize(StreamReader sw)
		{
			throw new NotImplementedException();
		}

		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			Writer w = new Writer(sw);
			// typedefs
			long ttType = w.WriteTypeDef("TransitionTime", "IsFixedDateRule", "Day", "Month", "TimeOfDay", "Week", "DayOfWeek").Current;
			long adjType = w.WriteTypeDef("Adjustment", "StartDate", "EndDate", "DaylightDeltaHours", "TransitionStart", "TransitionEnd").Current;
			long tzType = w.WriteTypeDef("TimeZone", "Id", "DisplayName", "DaylightName", "StandardName", "HasDst", "UtcOffsetHours", "Adjustments").Current;
			long tzsType = w.WriteTypeDef("TimeZones", "AllTimeZones").Current;

			int tzLen = ctzs.AllTimeZones.Length;
			long?[] rcTzsArr = new long?[tzLen];
			for (int tzCtr = 0; tzCtr < tzLen; tzCtr++)
			{
				CsnTimeZone ctz = ctzs.AllTimeZones[tzCtr];
				long? rcAdjArr = null;
				if (ctz.Adjustments != null)
				{
					int adjLen = ctz.Adjustments.Length;
					if (adjLen > 0)
					{
						long?[] arrAdjs = new long?[adjLen];
						for (int adjCtr = 0; adjCtr < adjLen; adjCtr++)
						{
							CsnAdjustment adj = ctz.Adjustments[adjCtr];
							long? rcTtStart = this.WriteTrTime(adj.TransitionStart, w, ttType);
							long? rcTtEnd = this.WriteTrTime(adj.TransitionEnd, w, ttType);
							arrAdjs[adjCtr] = w.WriteInstance(adjType).W(adj.StartDate).W(adj.EndDate).W(adj.DaylightDeltaHours).WRef(rcTtStart).WRef(rcTtEnd).Current;
						}

						rcAdjArr = w.WriteArray().WRef(arrAdjs).Current;
					}
				}
				rcTzsArr[tzCtr] = w.WriteInstance(tzType).W(ctz.Id).W(ctz.DisplayName).W(ctz.DaylightName).W(ctz.StandardName).W(ctz.HasDst).W(ctz.UtcOffsetHours).WRef(rcAdjArr).Current;
			}
			long rc = w.WriteArray().WRef(rcTzsArr).Current;
			w.WriteInstance(tzsType).WRef(rc);
			sw.Flush();
		}

		private long? WriteTrTime(CsnTransition tt, Writer w, long? ttType)
		{
			return (tt == null) ?
				(long?)null :
				w.WriteInstance(ttType.Value).W(tt.IsFixedDateRule).W(tt.Day).W(tt.Month).W(tt.TimeOfDay).W(tt.Week).W(tt.DayOfWeek).Current;
		}
	}
}
