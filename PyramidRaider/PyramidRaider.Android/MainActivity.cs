using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Content;
using System.Json;
using Android.Widget;
#if NOKIA_X
using Nokia.Payment.IAP;
#else
using Xamarin.InAppBilling;
#endif
using Mobi.Vserv.Android.Ads;
using OpenitvnGame;
using OpenitvnGame.Helpers;
using Android.Net;

namespace PyramidRaider
{
    [Activity(Label = "Pyramid Raider"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class MainActivity : Microsoft.Xna.Framework.AndroidGameActivity, IFeedbackCaller
    {
#if NOKIA_X
        //nokia in-app payment
        public const string IAP_PREMIUM_NOKIA = "1287826";
        INokiaIAPService nokiaIAPService;
        NokiaIAPServiceConnection nokiaIAPServiceConnection;
#else
        //google in-app billing
        public const string GOOGLE_PRODUCT_ID = "123456";
        InAppBillingServiceConnection googleIABServiceConnection;
#endif

        VservManager vserv;
        FrameLayout adView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            //display game
            var g = new Main();
            FrameLayout container = FindViewById<FrameLayout>(Resource.Id.container);
            container.AddView((View)g.Services.GetService(typeof(View)));
            g.Run();

            adView = FindViewById<FrameLayout>(Resource.Id.adView);
            adView.Visibility = ViewStates.Invisible;

            //prepare device's services
            prepareServices();
            vserv = VservManager.GetInstance(this);
        }

        public void RenderAd()
        {
            RelativeLayout.LayoutParams layoutParam = (RelativeLayout.LayoutParams)adView.LayoutParameters;
            switch (Resolution.CropMethod)
            {
                case ResolutionCropMethod.Overflow:
                case ResolutionCropMethod.Letterbox:
                    layoutParam.Width = (int)(Resolution.HardFactor * 480);
                    layoutParam.Height = (int)(Resolution.HardFactor * 110);
                    layoutParam.LeftMargin = (int)(60 * Resolution.HardFactor + Resolution.Viewpot.X);
                    layoutParam.TopMargin = (int)(258 * Resolution.HardFactor + Resolution.Viewpot.Y);
                    break;

                case ResolutionCropMethod.Softscale:
                    layoutParam.Width = (int)(Resolution.FactorWidth * 480);
                    layoutParam.Height = (int)(Resolution.FactorHeight * 110);
                    layoutParam.LeftMargin = (int)(60 * Resolution.FactorWidth);
                    layoutParam.TopMargin = (int)(258 * Resolution.FactorHeight);
                    break;
            }
            
            adView.LayoutParameters = layoutParam;
            adView.Visibility = ViewStates.Visible;

            vserv.SetShowAt(AdPosition.In);
            vserv.RenderAd("e41cf3d1", adView);
        }

        public void CloseAd()
        {
            adView.Visibility = ViewStates.Invisible;
        }

        public void DisplayAd()
        {
            vserv.SetShowAt(AdPosition.End);
            vserv.DisplayAd("c001db23", AdOrientation.Landscape);
        }

        private void prepareServices()
        {
#if NOKIA_X
            //prepare for nokia in-app payment
            // Create a connection to the service
            nokiaIAPServiceConnection = new Nokia.Payment.IAP.NokiaIAPServiceConnection(svc =>
            {
                // Set our instance of the service on the activity
                nokiaIAPService = svc;
            }, () =>
            {
                /* Disconnected */
                nokiaIAPService = null;
            });

            // Initiate binding of the service
            BindService(NokiaIAP.GetPaymentEnabler(), nokiaIAPServiceConnection, Bind.AutoCreate);
#else
                // Create a new connection to the Google Play Service
                googleIABServiceConnection = new InAppBillingServiceConnection(this, GOOGLE_PRODUCT_ID);
                googleIABServiceConnection.OnConnected += () =>
                {
                    // Load available products and any purchases
                };

                // Attempt to connect to the service
                googleIABServiceConnection.Connect();
#endif
        }

        public void RemoveAd()
        {
#if NOKIA_X
            // Trigger a purchase
            if (nokiaIAPService != null)
            {
                var intentBundle = nokiaIAPService.GetBuyIntent(NokiaIAP.IAP_VERSION, this.PackageName, IAP_PREMIUM_NOKIA, NokiaIAP.IAP_TYPE_INAPP, "");
                PendingIntent purchaseIntent = (PendingIntent)intentBundle.GetParcelable(NokiaIAP.BUY_INTENT);
                this.StartIntentSenderForResult(purchaseIntent.IntentSender, 0, new Intent(), 0, 0, 0);
            }
            else
            {
                Toast.MakeText(this, "Billing not available! Make sure you have a SIM card installed!", ToastLength.Short).Show();
            }
#endif
        }

        // We'll get a result from the Purchase process in this callback
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
#if NOKIA_X
            // -1 is the Nokia platform specific success code
            if ((int)resultCode == -1)
            {
                // Get the response code from the intent
                var responseCode = data.GetIntExtra(NokiaIAP.EXTRA_RESPONSE_CODE, -100);

                // Success response code?
                if (responseCode == 0)
                {
                    if (data.GetStringExtra(NokiaIAP.EXTRA_INAPP_PURCHASE_DATA) != null)
                    {
                        var json = JsonValue.Parse(data.GetStringExtra(NokiaIAP.EXTRA_INAPP_PURCHASE_DATA));

                        var productId = json["productId"];
                        var devPayload = json["developerPayload"];
                        var purchaseToken = json["purchaseToken"];

                        // TODO: Do something useful with the purchase confirmation!
                        Main.SwitchToPremium();
                        Toast.MakeText(this, "Purchase successed! Enjoy premium features!", ToastLength.Long).Show();
                    }
                }
                else
                {
                    // TODO: Something went wrong!
                    Toast.MakeText(this, "Purchase failed! Please try again later.", ToastLength.Long).Show();
                }
            }
#endif
        }

        public void Review()
        {
            string myUrl = "https://publish.nokia.com/content_items/show/569057";
            StartActivity(new Intent(Intent.ActionView, Uri.Parse(myUrl)));
        }


        public void Feedback()
        {
            
        }
    }
}

