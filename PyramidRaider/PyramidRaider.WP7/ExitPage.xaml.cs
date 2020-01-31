using System.Collections.Generic;
using Microsoft.Phone.Controls;
using Inneractive.Ad;
using Microsoft.Phone.Net.NetworkInformation;

namespace PyramidRaider
{
    public partial class ExitPage : PhoneApplicationPage
    {
        bool adCalled = false;

        public ExitPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Main.IsPremium || adCalled || !NetworkInterface.GetIsNetworkAvailable())
            {
                App.Quit();
            }
            else
            {
                InneractiveAd.DisplayAd("Openitvn_MummyMazeDeluxe_Nokia", InneractiveAd.IaAdType.IaAdType_Interstitial, LayoutRoot, 60, new Dictionary<InneractiveAd.IaOptionalParams, string>());
                adCalled = true;
            }
        }
		
		protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
			e.Cancel = true;
        }
    }
}