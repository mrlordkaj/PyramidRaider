using System.Collections.Generic;
using Microsoft.Phone.Controls;
using Inneractive.Ad;
using vservWindowsPhone;
using System.Windows;
using GoogleAds;
using Microsoft.Phone.Net.NetworkInformation;

namespace PyramidRaider
{
    public partial class ExitPage : PhoneApplicationPage
    {
        VservAdControl VAC;
        InterstitialAd admobInterstitial;

        bool adCalled = false;
        bool allowPressBack = false;
        public ExitPage()
        {
            InitializeComponent();

            VAC = VservAdControl.Instance;
            VAC.VservAdNoFill += VAC_VservAdNoFill;
            VAC.VservAdClosed += VAC_VservAdClosed;
            VAC.SetRequestTimeOut(30);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Main.IsPremium || adCalled || !NetworkInterface.GetIsNetworkAvailable())
            {
                Application.Current.Terminate();
            }
            else
            {
                admobInterstitial = new InterstitialAd("ca-app-pub-0642081750064354/5085232742");
                AdRequest adRequest = new AdRequest();
                admobInterstitial.FailedToReceiveAd += admobInterstitial_FailedToReceiveAd;
                admobInterstitial.LoadAd(adRequest);
                adCalled = true;
            }
        }

        void admobInterstitial_FailedToReceiveAd(object sender, AdErrorEventArgs e)
        {
            adCalled = false;
            VAC.DisplayAd("c001db23", LayoutRoot);
            adCalled = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if(allowPressBack) Application.Current.Terminate();
        }

        void VAC_VservAdClosed(object sender, System.EventArgs e)
        {
            Application.Current.Terminate();
        }

        private void VAC_VservAdNoFill(object sender, System.EventArgs e)
        {
            adCalled = false;
            InneractiveAd.DisplayAd("Openitvn_MummyMazeDeluxe_Nokia", InneractiveAd.IaAdType.IaAdType_Interstitial, LayoutRoot, 60, new Dictionary<InneractiveAd.IaOptionalParams, string>());
            adCalled = true;
            allowPressBack = true;
        }
    }
}