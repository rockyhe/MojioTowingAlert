using System;
using System.Collections.Generic;
using System.Linq;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.core
{
	public static class MyDataManager
	{
		private static readonly string logTag = "MyDataManager";

		#region UserPreferences

		public static UserPreference GetUserPreference (string userId)
		{
			MyLogger.Information (logTag, string.Format ("User Preferece: Retrieving {0}.", userId));
			var pref = MyDatabase.GetUserPreference (UserPreference.ToId (userId));
			MyLogger.Information (logTag, string.Format ("User Preferece: {0} Retrieved.", pref != null ? pref.UserId : "Not"));
			return pref;
		}

		public static bool SaveUserPreference (UserPreference item)
		{
			MyLogger.Information (logTag, string.Format ("User Preferece: Saving {0}.", item.UserId));
			MyDatabase.SaveUserPreference (item);
			var pref = GetUserPreference (item.UserId);
			MyLogger.Information (logTag, string.Format ("User Preferece: {0} Saved.", pref != null ? pref.UserId : "Not"));
			return pref != null;
		}

		#endregion

	}
}

