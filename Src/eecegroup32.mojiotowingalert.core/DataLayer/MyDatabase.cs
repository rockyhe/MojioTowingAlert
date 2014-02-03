using System;
using System.IO;
using SQLite;
using System.Collections.Generic;
using Mojio.Events;
using System.Linq;
using Mojio;

namespace eecegroup32.mojiotowingalert.core
{
	public class MyDatabase: SQLiteConnection
	{
		protected static MyDatabase myDB = null;
		protected static string dbPath;
		protected static object padLock = new object ();
		private static readonly string logTag = "MyDatabase";

		static MyDatabase ()
		{
			dbPath = GetDBPath ();
			myDB = new MyDatabase (dbPath);
		}

		protected MyDatabase (string path) : base (path)
		{		
			CreateTable<UserPreference> ();
			PrintOutTableSchema (GetMapping<UserPreference> ());
		}

		private void PrintOutTableSchema (TableMapping mapping)
		{
			var columns = mapping.Columns;
			MyLogger.Information (logTag, string.Format ("{0} Table Created. Columns: {1}", mapping.TableName, string.Join (", ", columns.Select (x => x.Name).ToArray ())));
		}

		private static string GetDBPath ()
		{
			var sqliteFilename = "MyDatabase.db3";        
			#if __ANDROID__
			string libraryPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			#else
			string documentsFolderPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); 
			string libraryFolderPath = Path.Combine (documentsPath, "..", "Library"); 
			#endif
			return Path.Combine (libraryPath, sqliteFilename);
		}

		#region UserPreferences

		public static IEnumerable<UserPreference> GetAllUserPreferences ()
		{
			lock (padLock) {
				return (from i in myDB.Table<UserPreference> ()
				        select i).ToList ();
			}
		}

		public static UserPreference GetUserPreference (int id)
		{
			lock (padLock) {
				MyLogger.Information (logTag, string.Format ("User Preferece: Retrieving {0}.", id));
				var pref = myDB.Table<UserPreference> ().FirstOrDefault (x => x.Id == id);
				MyLogger.Information (logTag, string.Format ("User Preferece: Returned {0}.", pref == null ? "Null" : string.Format ("{0}({1})", pref.UserId, pref.Id)));
				return pref;
			}
		}

		public static void SaveUserPreference (UserPreference item)
		{
			lock (padLock) {		
				MyLogger.Information (logTag, string.Format ("User Preferece: Saving {0}.", item.UserId));
				var pref = myDB.Table<UserPreference> ().FirstOrDefault (x => x.Id == item.Id);
				if (pref == null) {
					myDB.Insert (item);
					MyLogger.Information (logTag, string.Format ("User Preferece: {0} Inserted.", item.UserId));
				} else {					
					myDB.Update (item);
					MyLogger.Information (logTag, string.Format ("User Preferece: {0} Updated.", item.UserId));
				}
			}
		}

		public static void DeleteUserPreference (int id)
		{
			lock (padLock) {
				MyLogger.Information (logTag, string.Format ("User Preferece: Deleting {0}.", id));
				myDB.Delete<UserPreference> (id);
			}
		}

		#endregion

	}
}
