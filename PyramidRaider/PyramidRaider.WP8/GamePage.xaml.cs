using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MonoGame.Framework.WindowsPhone;
using vservWindowsPhone;
using Inneractive.Ad;
using Windows.ApplicationModel.Store;
using GoogleAds;

namespace PyramidRaider
{
    public partial class GamePage : PhoneApplicationPage
    {
        const string IAP_PREMIUM = "pyramid_raider_premium";

        private Main _game;
        public static GamePage Instance = null;

        VservAdControl vservAdControl;
        InneractiveAd inneractiveBanner;
        AdView admobBanner;
        AdRequest admobRequest;
        public bool AppLoaded { get; private set; }
        public bool AdLoaded { get; private set; }

        // Constructor
        public GamePage()
        {
            AppLoaded = false;

            InitializeComponent();

            if (Instance != null) throw new InvalidOperationException("There can be only one GamePage object!");
            Instance = this;

            this.Loaded += GamePage_Loaded;
            adView.Visibility = Visibility.Collapsed;

            //vserv ad
            vservAdControl = VservAdControl.Instance;
            vservAdControl.VservAdNoFill += vservAdControl_VservAdNoFill;
            vservAdControl.SetRefreshRate(30);
            vservAdControl.SetRequestTimeOut(30);
            //end vserv ad

            //google admob
            admobBanner = new AdView
            {
                Format = AdFormats.Banner,
                AdUnitID = "ca-app-pub-0642081750064354/3748100349"
            };
            admobBanner.ReceivedAd += admobBanner_ReceivedAd;
            admobBanner.FailedToReceiveAd += admobBanner_FailedToReceiveAd;
            admobRequest = new AdRequest();
            //google admob

            _game = XamlGame<Main>.Create("", this);
        }

        void vservAdControl_VservAdNoFill(object sender, EventArgs e)
        {
            adView.Children.Clear();
            inneractiveBanner = new InneractiveAd("Openitvn_MummyMazeDeluxe_Nokia", InneractiveAd.IaAdType.IaAdType_Banner, 60, new Dictionary<InneractiveAd.IaOptionalParams, string>());
            adView.Children.Add(inneractiveBanner);
        }

        void admobBanner_ReceivedAd(object sender, AdEventArgs e)
        {
            AdLoaded = true;
        }

        void admobBanner_FailedToReceiveAd(object sender, AdErrorEventArgs e)
        {
            adView.Children.Clear();
            vservAdControl.RenderAd("e41cf3d1", adView);
            AdLoaded = true;
        }

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            AppLoaded = true;
            
            if (!Main.IsPremium && CurrentApp.LicenseInformation.ProductLicenses[IAP_PREMIUM].IsActive)
            {
                Main.SwitchToPremium();
            }
        }

        public void RemoveAd()
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    var x = CurrentApp.RequestProductPurchaseAsync(IAP_PREMIUM, false);
                    await x;
                    if (x.Status == Windows.Foundation.AsyncStatus.Completed)
                    {
                        Main.SwitchToPremium();
                        CloseAd();
                    }
                }
                catch { }
            });
        }

        public void RenderAd()
        {
            if (AppLoaded)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    AdLoaded = false;
                    adView.Children.Clear();
                    adView.Visibility = Visibility.Visible;
                    
                    adView.Children.Add(admobBanner);
                    admobBanner.LoadAd(admobRequest);
                });
            }
        }

        public void CloseAd()
        {
            Dispatcher.BeginInvoke(() =>
            {
                adView.Visibility = Visibility.Collapsed;
            });
        }

        public void GotoExitPage()
        {
            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/ExitPage.xaml", UriKind.Relative));
            });
        }
    }
}