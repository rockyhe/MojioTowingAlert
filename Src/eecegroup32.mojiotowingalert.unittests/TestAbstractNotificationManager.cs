using System;
using eecegroup32.mojiotowingalert.android;
using eecegroup32.mojiotowingalert.core;
using Android.Test;
using Android.Test.Mock;
using Android.OS;
using Mojio.Events;
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
			notificationManager = new TowNotificationManager ();

		}

		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void TestClearNotificaitons ()
		{
			notificationManager.Clear ();
			Assert.AreEqual (0, notificationManager.GetNewEventNumber());
		}

		[Test]
		public void TestIncrementNotifications ()
		{
			notificationManager.Clear ();
			notificationManager.IncrementNewEventNumber ();
			Assert.AreEqual (1,notificationManager.GetNewEventNumber());
		}

		[Test]
		public void TestAddEvent ()
		{
			notificationManager.Clear ();
			Event mojioEvent = new Event ();
			Assert.True(notificationManager.Add(mojioEvent));
		}

		[Test]
		public void TestFindEvent ()
		{
			notificationManager.Clear ();
			Event mojioEvent = new Event ();
			System.Guid id = new System.Guid ();
			mojioEvent.OwnerId = id;
			notificationManager.Add (mojioEvent);
			Assert.AreEqual (mojioEvent, notificationManager.Find(id.ToString()));
		}
	}
}

