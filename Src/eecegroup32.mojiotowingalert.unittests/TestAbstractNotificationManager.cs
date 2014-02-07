using System;
using eecegroup32.mojiotowingalert.android;
using eecegroup32.mojiotowingalert.core;
using Android.Test;
using Android.Test.Mock;
using Android.OS;
using NUnit.Framework;

namespace eecegroup32.mojiotowingalert.unittests
{
	[TestFixture]
	public class TestAbstractNotificationManager
	{
		private AbstractNotificationManager notificationManager;


		[SetUp]
		public void Setup ()
		{
			notificationManager = new AbstractNotificationManager ();

		}

		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void TestClearNotificaitons ()
		{
			notificationManager.Clear ();
			Assert.Equals (notificationManager.GetNewEventNumber(), 0);
		}

		[Test]
		public void TestIncrementNotifications ()
		{
			notificationManager.Clear ();
			notificationManager.IncrementNewEventNumber ();
			Assert.Equals (notificationManager.GetNewEventNumber(), 1);
		}

		[Test]
		public void Ignore ()
		{
			Assert.True(true);
		}

		[Test]
		public void Inconclusive ()
		{
			Assert.Inconclusive ("Inconclusive");
		}
	}
}

