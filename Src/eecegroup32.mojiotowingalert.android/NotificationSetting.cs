using System;
using System.Collections.Generic;

namespace eecegroup32.mojiotowingalert.android
{
	//CAPSTONE
	public class NotificationSetting
	{
		public Boolean OnOff { get; set; }
		public Boolean Vibration { get; set; }
		public Boolean Sound { get; set; }
		public override string ToString ()
		{
			return string.Format ("[NotificationSetting: OnOff={0}, Vibration={1}, Sound={2}]", OnOff, Vibration, Sound);
		}

		public void ParseToString(String toString) {
			String[] options = toString.Split (',');
			OnOff = options [0].Contains ("true");
			Vibration = options [0].Contains ("true");
			Sound = options [0].Contains ("true");
		}
	}
}

