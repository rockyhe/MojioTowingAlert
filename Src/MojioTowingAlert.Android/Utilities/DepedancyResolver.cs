using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MojioTowingAlert.Android
{
	class DependancyResolver
	{
		static object locker = new object ();
		static DependancyResolver instance;

		private DependancyResolver ()
		{
			Services = new Dictionary<Type, Lazy<object>> ();
		}

		private Dictionary<Type, Lazy<object>> Services { get; set; }

		private static DependancyResolver Instance
		{
			get
			{
				lock (locker) {
					if (instance == null)
						instance = new DependancyResolver ();
					return instance;
				}
			}
		}

		public static void Set<T> (T service)
		{
			Instance.Services [typeof (T)] = new Lazy<object> (() => service);
		}

		public static void Set<T> ()
			where T : new ()
		{
			Instance.Services [typeof (T)] = new Lazy<object> (() => new T ());
		}

		public static void Set<T> (Func<object> function)
		{
			Instance.Services [typeof (T)] = new Lazy<object> (function);
		}

		public static T Get<T> ()
		{
			Lazy<object> service;
			if (Instance.Services.TryGetValue (typeof (T), out service)) {
				return (T)service.Value;
			} else {
				throw new KeyNotFoundException (string.Format ("Service not found for type '{0}'", typeof (T)));
			}
		}
	}
}

