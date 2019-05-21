using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace Performance
{
	class JsonSer
		: ISerializer
	{
		public static readonly string[] shortNames = new string[]
		{
			"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S",
		};
		public static readonly string[] longNames = new string[]
		{
			"AllTimeZones",
			"Id", "DisplayName", "DaylightName", "StandardName", "HasDst", "UtcOffsetHours", "Adjustments",
			"StartDate", "EndDate", "DaylightDeltaHours", "TransitionStart", "TransitionEnd",
			"IsFixedDateRule", "Day", "Month", "TimeOfDay", "Week", "DayOfWeek",
		};

		private string[] names = null;

		public JsonSer(string[] pNames)
		{
			this.names = pNames;
		}


		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			//JsonSerializer json = new JsonSerializer();
			//json.Serialize(sw, ctzs);

			JsonTextWriter json = new JsonTextWriter(sw);
			Serialize(ctzs, json);
		}

		private void Serialize(CsnTimeZones ctzs, JsonTextWriter json)
		{
			json.WriteStartObject();
			json.WritePropertyName(this.names[0]);
			json.WriteStartArray();
			foreach (CsnTimeZone ctz in ctzs.AllTimeZones)
			{
				Serialize(ctz, json);
			}
			json.WriteEndArray();
			json.WriteEndObject();
		}

		private void Serialize(CsnTimeZone ctz, JsonTextWriter json)
		{
			json.WriteStartObject();
			json.WritePropertyName(this.names[1]);
			json.WriteValue(ctz.Id);
			json.WritePropertyName(this.names[2]);
			json.WriteValue(ctz.DisplayName);
			json.WritePropertyName(this.names[3]);
			json.WriteValue(ctz.DaylightName);
			json.WritePropertyName(this.names[4]);
			json.WriteValue(ctz.StandardName);
			json.WritePropertyName(this.names[5]);
			json.WriteValue(ctz.HasDst);
			json.WritePropertyName(this.names[6]);
			json.WriteValue(ctz.UtcOffsetHours);

			if (ctz.Adjustments != null) {
				json.WritePropertyName(this.names[7]);
				json.WriteStartArray();
				foreach (CsnAdjustment adj in ctz.Adjustments)
				{
					Serialize(adj, json);
				}
				json.WriteEndArray();
			}

			json.WriteEndObject();
		}

		private void Serialize(CsnAdjustment adj, JsonTextWriter json)
		{
			json.WriteStartObject();
			json.WritePropertyName(this.names[8]);
			json.WriteValue(adj.StartDate);
			json.WritePropertyName(this.names[9]);
			json.WriteValue(adj.EndDate);
			json.WritePropertyName(this.names[10]);
			json.WriteValue(adj.DaylightDeltaHours);
			Serialize(this.names[11], adj.TransitionStart, json);
			Serialize(this.names[12], adj.TransitionEnd, json);
			json.WriteEndObject();
		}

		private void Serialize(string name, CsnTransition tt, JsonTextWriter json)
		{
			if (tt != null)
			{
				json.WritePropertyName(name);
				json.WriteStartObject();
				json.WritePropertyName(this.names[13]);
				json.WriteValue(tt.IsFixedDateRule);
				json.WritePropertyName(this.names[14]);
				json.WriteValue(tt.Day);
				json.WritePropertyName(this.names[15]);
				json.WriteValue(tt.Month);
				json.WritePropertyName(this.names[16]);
				json.WriteValue(tt.TimeOfDay);
				json.WritePropertyName(this.names[17]);
				json.WriteValue(tt.Week);
				json.WritePropertyName(this.names[18]);
				json.WriteValue(tt.DayOfWeek);
				json.WriteEndObject();
			}
		}
	}
}
