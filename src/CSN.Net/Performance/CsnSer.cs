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
		public CsnTimeZones Deserialize(StreamReader sw)
		{
			throw new NotImplementedException();
		}

		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			Writer w = new Writer(sw);
			// typedefs
			RecordCode  ttType = w.WriteTypeDef("TransitionTime", "IsFixedDateRule", "Day", "Month", "TimeOfDay", "Week", "DayOfWeek").Current;
			RecordCode  adjType = w.WriteTypeDef("Adjustment", "StartDate", "EndDate", "DaylightDeltaHours", "TransitionStart", "TransitionEnd").Current;
			RecordCode  tzType = w.WriteTypeDef("TimeZone", "Id", "DisplayName", "DaylightName", "StandardName", "HasDst", "UtcOffsetHours", "Adjustments").Current;
			RecordCode  tzsType = w.WriteTypeDef("TimeZones", "AllTimeZones").Current;

			int tzLen = ctzs.AllTimeZones.Length;
			RecordCode [] rcTzsArr = new RecordCode [tzLen];
			for (int tzCtr = 0; tzCtr < tzLen; tzCtr++)
			{
				CsnTimeZone ctz = ctzs.AllTimeZones[tzCtr];
				RecordCode  rcAdjArr = null;
				if (ctz.Adjustments != null)
				{
					int adjLen = ctz.Adjustments.Length;
					if (adjLen > 0)
					{
						RecordCode [] arrAdjs = new RecordCode [adjLen];
						for (int adjCtr = 0; adjCtr < adjLen; adjCtr++)
						{
							CsnAdjustment adj = ctz.Adjustments[adjCtr];
							RecordCode  rcTtStart = this.WriteTrTime(adj.TransitionStart, w, ttType);
							RecordCode  rcTtEnd = this.WriteTrTime(adj.TransitionEnd, w, ttType);
							arrAdjs[adjCtr] = w.WriteInstance(adjType).W(adj.StartDate).W(adj.EndDate).W(adj.DaylightDeltaHours).W(rcTtStart).W(rcTtEnd).Current;
						}

						rcAdjArr = w.WriteArray(adjType).W(arrAdjs).Current;
					}
				}
				rcTzsArr[tzCtr] = w.WriteInstance(tzType).W(ctz.Id).W(ctz.DisplayName).W(ctz.DaylightName).W(ctz.StandardName).W(ctz.HasDst).W(ctz.UtcOffsetHours).W(rcAdjArr).Current;
			}
			RecordCode  rc = w.WriteArray(tzType).W(rcTzsArr).Current;
			w.WriteInstance(tzsType).W(rc);
			sw.Flush();
		}

		private RecordCode  WriteTrTime(CsnTransition tt, Writer w, RecordCode  ttType)
		{
			return (tt == null) ?
				null :
				w.WriteInstance(ttType).W(tt.IsFixedDateRule).W(tt.Day).W(tt.Month).W(tt.TimeOfDay).W(tt.Week).W(tt.DayOfWeek).Current;
		}
	}
}
