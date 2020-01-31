using System;
using System.Windows.Navigation;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Inneractive.Ad;

namespace PyramidRaider
{
    public partial class GamePage : PhoneApplicationPage
    {
        const string IAP_PREMIUM = "pyramid_raider_premium";

        public static GamePage Instance = null;

        public bool AppLoaded { get; private set; }

        public UIElementRenderer UiElementRenderer { get; private set; }

        GameTimer timer;
        Main game;

        public GamePage()
        {
            if (Instance != null) throw new InvalidOperationException("There can be only one GamePage object!");
            Instance = this;

            InitializeComponent();

            game = new Main();
            game.CInitialize();

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            adView.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // TODO: use this.content to load your game content here
            if (!AppLoaded)
            {
                game.CLoadContent();
                AppLoaded = true;
            }

            UiElementRenderer = new UIElementRenderer(LayoutRoot,
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width,
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height);

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            UiElementRenderer.Dispose();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            game.PerformBack();
            e.Cancel = true;
        }

        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            UiElementRenderer.Render();
            game.CUpdate(new GameTime(e.TotalTime, e.ElapsedTime));
        }

        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            game.CDraw(new GameTime(e.TotalTime, e.ElapsedTime));
        }

        public void RenderAd()
        {
            if (AppLoaded)
            {
                adView.Visibility = Visibility.Visible;
                InneractiveAd iaBanner = new InneractiveAd("Openitvn_MummyMazeDeluxe_Nokia", InneractiveAd.IaAdType.IaAdType_Banner, 60, new System.Collections.Generic.Dictionary<InneractiveAd.IaOptionalParams, string>());
                adView.Children.Add(iaBanner);
            }
        }

        public void CloseAd()
        {
            adView.Visibility = Visibility.Collapsed;
        }

        public void GotoExitPage()
        {
            NavigationService.Navigate(new Uri("/ExitPage.xaml", UriKind.Relative));
        }
    }
}