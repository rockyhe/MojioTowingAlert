using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using PushSharp.Client;
using Mojio.Events;
using Mojio.Client;
using Android.OS;
using eecegroup32.mojiotowingalert.core;

[assembly: Permission (Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission (Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission (Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission (Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission (Name = "android.permission.INTERNET")]
[assembly: UsesPermission (Name = "android.permission.WAKE_LOCK")]
namespace eecegroup32.mojiotowingalert.android
{
	[BroadcastReceiver (Permission = GCMConstants.PERMISSION_GCM_INTENTS)]
	[IntentFilter (new string[] { GCMConstants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter (new string[] { GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter (new string[] { GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
	public class PushReceiver : PushHandlerBroadcastReceiverBase<PushService>
	{
		public static string[] SENDER_IDS = new string[] { "617048402524" };
		public const string TAG = "PushService";
	}

	public abstract class EventReceiver : BroadcastReceiver
	{
		public static readonly string IntentAction = "MojioEvent";

		public override void OnReceive (Context context, Intent intent)
		{
			MyLogger.Information ("EventReceiver", "Event JSON: Received.");
			var json = intent.GetStringExtra ("data");
			MyLogger.Information ("EventReceiver", string.Format ("Event JSON: Deserialized. {0}", json));
			if (!string.IsNullOrEmpty (json)) {
				var deserializedEvent = MojioClient.Deserialize<Event> (json);
				if (deserializedEvent != null) {
					OnEvent (context, deserializedEvent);
					MyLogger.Information ("EventReceiver", string.Format ("Deserialized Event: Type {0}", deserializedEvent.EventType));
				} else
					MyLogger.Error ("EventReceiver", string.Format ("Deserialized Event: Null"));
			} else {
				MyLogger.Error ("EventReceiver", "Event JSON: Empty or null.");
			}
		}

		protected abstract void OnEvent (Context context, Event ev);
	}

	[Service]
	public class PushService : PushHandlerServiceBase
	{
		public PushService () : base (PushReceiver.SENDER_IDS)
		{
		}

		protected override void OnRegistered (Context context, string registrationId)
		{
			MyLogger.Information ("PushService", string.Format ("PushService Registered."));
		}

		protected override void OnUnRegistered (Context context, string registrationId)
		{
			MyLogger.Information ("PushService", string.Format ("PushService Unregistered."));
		}

		protected override void OnMessage (Context context, Intent intent)
		{
			MyLogger.Information ("PushService", "Push Message: Received");
			
			if (intent != null && intent.GetStringExtra ("type") == "MojioEvent") {
				MyLogger.Information ("PushService", string.Format ("Push Message: {0}", intent == null ? "false message" : "type " + intent.GetStringExtra ("type")));
				var broadcast = new Intent ();
				broadcast.PutExtras (intent);
				broadcast.SetAction (EventReceiver.IntentAction);
				SendBroadcast (broadcast);
			} else {
				MyLogger.Information ("PushService", "Push Message: Not Mojio Event");
			}
		}

		protected override bool OnRecoverableError (Context context, string errorId)
		{
			MyLogger.Error ("PushService", "OnRecoverableError invoked.");
			return base.OnRecoverableError (context, errorId);
		}

		protected override void OnError (Context context, string errorId)
		{
			MyLogger.Error ("PushService", "OnError invoked.");
		}
	}
}