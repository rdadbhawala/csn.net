using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Performance
{
    public class CsnTimeZones
    {
		public CsnTimeZones()
		{ }

		public CsnTimeZone[] TimeZones { get; set; }
    }

	public class CsnTimeZone
	{
		public CsnTimeZone()
		{ }

		public CsnTimeZone(TimeZoneInfo tzi)
		{
			this.Id = tzi.Id;
			this.StandardName = tzi.StandardName;
			this.DisplayName = tzi.DisplayName;
			this.DaylightName = tzi.DaylightName;
			this.HasDst = tzi.SupportsDaylightSavingTime;
			this.Adjustments = tzi.GetAdjustmentRules().Select(x => new CsnAdjustment(x)).ToArray();
			this.UtcOffsetHours = tzi.BaseUtcOffset.TotalHours;
		}

		public string Id { get; set; }
		public string DisplayName { get; set; }
		public string DaylightName { get; set; }
		public string StandardName { get; set; }
		public bool HasDst { get; set; }
		public double UtcOffsetHours { get; set; }

		public CsnAdjustment[] Adjustments { get; set; }
	}

	public class CsnAdjustment
	{
		public CsnAdjustment()
		{ }

		public CsnAdjustment(TimeZoneInfo.AdjustmentRule rule)
		{
			this.StartDate = rule.DateStart;
			this.EndDate = rule.DateEnd;
			this.DaylightDeltaHours = rule.DaylightDelta.TotalHours;
			this.TransitionStart = new CsnTransition(rule.DaylightTransitionStart);
			this.TransitionEnd = new CsnTransition(rule.DaylightTransitionEnd);
		}

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public double DaylightDeltaHours { get; set; }
		public CsnTransition TransitionStart { get; set; }
		public CsnTransition TransitionEnd { get; set; }
	}

	public class CsnTransition
	{
		public CsnTransition()
		{ }

		public CsnTransition(TimeZoneInfo.TransitionTime tt)
		{
			this.IsFixedDateRule = tt.IsFixedDateRule;
			this.Month = tt.Month;
			this.Day = tt.Day;
			this.Week = tt.Week;
			this.DayOfWeek = (int)tt.DayOfWeek;
			this.TimeOfDay = tt.TimeOfDay;
		}

		public bool IsFixedDateRule { get; set; }
		public int Day { get; set; }
		public int Month { get; set; }
		public DateTime TimeOfDay { get; set; }
		public int Week { get; set; }
		public int DayOfWeek { get; set; }
	}
}
