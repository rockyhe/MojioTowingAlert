using System.Reflection;
using Android.App;
using Android.Test;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace eecegroup32.mojiotowingalert.unittests
{
	[Activity (Label = "eecegroup32.mojiotowingalert.unittests", MainLauncher = true)]
	public class MainActivity : TestSuiteActivity
	{
		private InstrumentationTestRunner runner;

		protected override void OnCreate (Bundle bundle)
		{
			runner = new InstrumentationTestRunner ();
			// tests can be inside the main assembly
			AddTest (Assembly.GetExecutingAssembly ());
			// or in any reference assemblies
			// AddTest (typeof (Your.Library.TestClass).Assembly);

			// Once you called base.OnCreate(), you cannot add more assemblies.
			base.OnCreate (bundle);
		}
	}
}

