using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using PushSharp.Client;
using Mojio.Events;
using Mojio.Client;
using Android.OS;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace eecegroup32.mojiotowingalert.android
{
    [BroadcastReceiver(Permission = GCMConstants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushReceiver : PushHandlerBroadcastReceiverBase<PushService>
    {       
		public static string[] SENDER_IDS = new string[] { "617048402524" };
        public const string TAG = "PushService";
    }

    public abstract class EventReceiver : BroadcastReceiver
    {
		private ILogger logger = MainApp.Logger;
		public static readonly string IntentAction = "MojioEvent";

        public override void OnReceive(Context context, Intent intent)
        {
			logger.Information ("EventReceiver", "Event JSON: Received.");
            var json = intent.GetStringExtra("data");

            if (!string.IsNullOrEmpty(json))
            {
                var ev = MojioClient.Deserialize<Event>(json);
				logger.Information ("EventReceiver", string.Format ("Event JSON: Deserialized. {0}", ev.EventType));
                if (ev != null)
                    OnEvent(context,ev);
            }
        }

        protected abstract void OnEvent(Context context, Event ev);
    }

    [Service]
    public class PushService : PushHandlerServiceBase
    {
		private ILogger logger = MainApp.Logger;
	
		public PushService() : base(PushReceiver.SENDER_IDS) { }

        protected override void OnRegistered(Context context, string registrationId) { }

		protected override void OnUnRegistered(Context context, string registrationId) { }

        protected override void OnMessage(Context context, Intent intent)
        {
			logger.Information ("PushService", "Push Message: Received");
			logger.Information ("PushService", string.Format ("Push Message: {0}", intent == null ? "false message": "type " + intent.GetStringExtra("type")));
            if (intent != null && intent.GetStringExtra("type") == "MojioEvent")
            {
                var broadcast = new Intent();
                broadcast.PutExtras(intent);
                broadcast.SetAction(EventReceiver.IntentAction);
                SendBroadcast(broadcast);
            }
        }

        protected override bool OnRecoverableError(Context context, string errorId)
        {
            return base.OnRecoverableError(context, errorId);
        }

        protected override void OnError(Context context, string errorId) { }
    }
}