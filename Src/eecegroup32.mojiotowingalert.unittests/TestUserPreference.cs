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
	public class TestUserPreference
	{
		private UserPreference userPreference;


		[SetUp]
		public void Setup ()
		{
			userPreference = new UserPreference ();
		}

		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void TestGetAndSetUserID ()
		{
			string userID = "123";
			userPreference.UserId = userID;
			Assert.AreEqual (userID, userPreference.UserId);
		}

		[Test]
		public void TestGetAndSetNotificationChecked ()
		{
			bool check = false;
			userPreference.NotificationChecked = check;
			Assert.AreEqual (check , userPreference.NotificationChecked);
		}

		[Test]
		public void TestGetAndSetSoundChecked ()
		{
			bool check = false;
			userPreference.SoundChecked = check;
			Assert.AreEqual (check , userPreference.SoundChecked);
		}

		[Test]
		public void TestGetAndSetVibrationChecked ()
		{
			bool check = false;
			userPreference.VibrationChecked = check;
			Assert.AreEqual (check , userPreference.VibrationChecked);
		}



	}
}

