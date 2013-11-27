using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using PushSharp.Client;
using Mojio.Events;
using Mojio.Client;
using Android.OS;

//VERY VERY VERY IMPORTANT NOTE!!!!
// Your package name MUST NOT start with an uppercase letter.
// Android does not allow permissions to start with an upper case letter
// If it does you will get a very cryptic error in logcat and it will not be obvious why you are crying!
// So please, for the love of all that is kind on this earth, use a LOWERCASE first letter in your Package Name!!!!
[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")] //, ProtectionLevel = Android.Content.PM.Protection.Signature)]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace eecegroup32.mojiotowingalert.android
{
    //You must subclass this!
    [BroadcastReceiver(Permission = GCMConstants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushReceiver : PushHandlerBroadcastReceiverBase<PushService>
    {
        //IMPORTANT: Change this to your own Sender ID!
        //The SENDER_ID is your Google API Console App Project ID.
        //  Be sure to get the right Project ID from your Google APIs Console.  It's not the named project ID that appears in the Overview,
        //  but instead the numeric project id in the url: eg: https://code.google.com/apis/console/?pli=1#project:785671162406:overview
        //  where 785671162406 is the project id, which is the SENDER_ID to use!
		public static string[] SENDER_IDS = new string[] { "25361329364" };

        public const string TAG = "PushService";
    }

    public abstract class EventReceiver : BroadcastReceiver
    {
        public const string IntentAction = "MojioEvent";

        public override void OnReceive(Context context, Intent intent)
        {
            var json = intent.GetStringExtra("data");
            if (!string.IsNullOrEmpty(json))
            {
				// Deserialize using Mojio's Deserializer
                var ev = MojioClient.Deserialize<Event>(json);

                if (ev != null)
                    OnEvent(context,ev);
            }
        }

        protected abstract void OnEvent(Context context, Event ev);
    }

    /*
	[BroadcastReceiver]
    [IntentFilter(new string[] { EventReceiver.IntentAction })]
    public class TestReceiver : EventReceiver
    {
        protected override void OnEvent(Context context, Event ev)
        {
            // Process event (ev)
        }
    }
    */

    [Service] //Must use the service tag
    public class PushService : PushHandlerServiceBase
    {
	
        public PushService() : base(PushReceiver.SENDER_IDS) { 

		}

        protected override void OnRegistered(Context context, string registrationId)
        {

        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {

        }

        protected override void OnMessage(Context context, Intent intent)
        {

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

        protected override void OnError(Context context, string errorId)
        {
         
        }
    }
}