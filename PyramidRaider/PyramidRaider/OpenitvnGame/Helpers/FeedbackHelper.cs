using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
#if WINDOWS_PHONE
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
#endif

namespace OpenitvnGame.Helpers
{
    public enum FeedbackState
    {
        Inactive,
        FirstReview,
        SecondReview,
        Feedback
    }

    /// <summary>
    /// This helper class controls the behaviour of the FeedbackOverlay control
    /// When the app has been launched 5 times the initial prompt is shown
    /// If the user reviews no more prompts are shown
    /// When the app has bee launched 10 times and not been reviewed, the prompt is shown
    /// </summary>
    public class FeedbackHelper
    {
        private const string LAUNCH_COUNT = "feedback_launch_count";
        private const string REVIEWED = "feedback_reviewed";
        private const int FIRST_COUNT = 5;
        private const int SECOND_COUNT = 10;

        private int _launchCount = 0;
        private bool _reviewed = false;

        public static readonly FeedbackHelper Default = new FeedbackHelper();

        private FeedbackState _state = FeedbackState.Inactive;

        public FeedbackState State
        {
            get { return this._state; }
            set { this._state = value; }
        }

        private string Message { get; set; }
        private string Title { get; set; }
        private string YesText { get; set; }
        private string NoText { get; set; }

        private FeedbackHelper()
        {

        }

        /// <summary>
        /// Loads last state from storage and works out the new state
        /// </summary>
        private void LoadState()
        {
            try
            {
                this._launchCount = SettingHelper.GetSetting<int>(LAUNCH_COUNT, default(int));
                this._reviewed = SettingHelper.GetSetting<bool>(REVIEWED, default(bool));

                if (!this._reviewed)
                {
                    this._launchCount++;

                    if (this._launchCount == FIRST_COUNT)
                        this._state = FeedbackState.FirstReview;
                    else if (this._launchCount == SECOND_COUNT)
                        this._state = FeedbackState.SecondReview;

                    this.StoreState();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("FeedbackHelper.LoadState - Failed to load state, Exception: {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Stores current state
        /// </summary>
        private void StoreState()
        {
            try
            {
                SettingHelper.StoreSetting(LAUNCH_COUNT, this._launchCount, true);
                SettingHelper.StoreSetting(REVIEWED, this._reviewed, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("FeedbackHelper.StoreState - Failed to store state, Exception: {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Call when user has reviewed
        /// </summary>
        public void Reviewed()
        {
            this._reviewed = true;

            this.StoreState();
        }

        public IFeedbackCaller Caller { get; private set; }

        public void Initialise(IFeedbackCaller caller)
        {
            Caller = caller;

            // Only load state if not trial
            if (!Microsoft.Xna.Framework.GamerServices.Guide.IsTrialMode)
            {
                this.LoadState();

                // Uncomment for testing
                // this._state = FeedbackState.FirstReview;
                // this._state = FeedbackState.SecondReview;

                if (this.State == FeedbackState.FirstReview)
                {
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "vi")
                    {
                        this.Title = "Bạn có thích trò này?";
                        this.Message = "Nhóm mình luôn lắng nghe ý kiến người chơi để có thể hoàn thiện sản phẩm hơn.\r\n\r\nNếu bạn thấy thích trò chơi này, hãy dành 2 phút để đánh giá 5 sao và gửi lời nhận xét hoặc góp ý của bạn nhé!";
                        this.YesText = "bầu 5 sao";
                        this.NoText = "không muốn";
                    }
                    else
                    {
                        this.Title = "Enjoying this game?";
                        this.Message = "We'd love you to rate our app 5 stars\r\n\r\nShowing us some love on the store helps us to continue to work on the app and make things even better!";
                        this.YesText = "rate 5 stars";
                        this.NoText = "no thanks";
                    }

                    this.ShowMessage();
                }
                else if (this.State == FeedbackState.SecondReview)
                {
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "vi")
                    {
                        this.Title = "Bạn có thích trò này?";
                        this.Message = "Hình như là bạn khá thích trò chơi này!\r\n\r\nBạn sẽ dành ra 2 phút để đánh giá 5 sao và gửi nhận xét hoặc góp ý cho nhóm mình chứ?";
                        this.YesText = "bầu 5 sao";
                        this.NoText = "không muốn";
                    }
                    else
                    {
                        this.Title = "Enjoying this game?";
                        this.Message = "You look to be getting a lot of use out of our application!\r\n\r\nWhy not give us a 5 star rating to show your appreciation?";
                        this.YesText = "rate 5 stars";
                        this.NoText = "no thanks";
                    }

                    this.ShowMessage();
                }
            }
        }

        private void ShowMessage()
        {
            int loop = 0;

            // Check guide is not open
            while (Guide.IsVisible)
            {
                if (loop > 20) // Max 2s
                    return;
                loop++;

                System.Threading.Thread.Sleep(100);
            }

            Guide.BeginShowMessageBox(this.Title, this.Message,
                new List<string>() { this.YesText, this.NoText },
                0, MessageBoxIcon.None, (r) =>
                    {
                        var result = Guide.EndShowMessageBox(r);
                        if (result.HasValue && result.Value == 0)
                            OnYesClick();
                        else
                            OnNoClick();

                    }, null);
        }

        private void OnNoClick()
        {
            if (FeedbackHelper.Default.State != FeedbackState.Feedback)
            {
                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "vi")
                {

                    this.Title = "Cần làm tốt hơn?";
                    this.Message = "Thật buồn là bạn không muốn đánh giá trò chơi này.\r\n\r\n" +
                        "Nếu bạn thấy chúng mình cần nỗ lực hơn nữa, bạn có thể gửi email riêng tư để góp ý cho nhóm.";
                    this.YesText = "phản hồi";
                    this.NoText = "không muốn";
                }
                else
                {
                    this.Title = "Can we make it better?";
                    this.Message = "Sorry to hear you didn't want to rate MyApp.\r\n\r\n" +
                        "Tell us about your experience or suggest how we can make it even better.";
                    this.YesText = "give feedback";
                    this.NoText = "no thanks";
                }

                FeedbackHelper.Default.State = FeedbackState.Feedback;
                ShowMessage();
            }
        }

        private void OnYesClick()
        {
            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                this.Review();
            }
            else if (FeedbackHelper.Default.State == FeedbackState.SecondReview)
            {
                this.Review();
            }
#if !ANDROID
            else if (FeedbackHelper.Default.State == FeedbackState.Feedback)
            {
                this.Feedback();
            }
#endif
        }

        private void Review()
        {
            this.Reviewed();

            Caller.Review();
#if WINDOWS_PHONE
            var marketplace = new MarketplaceReviewTask();
            marketplace.Show();
#endif
        }

#if !ANDROID
        private void Feedback()
        {
            // Application version
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            var version = parts[1].Split('=')[1];

            // Body text including hardware, firmware and software info
            string body = string.Format(
                "[Your feedback here]\r\n\r\n\r\n" +
                "---------------------------------\r\n" +
                "Device Name: {0}\r\n" +
                "Device Manufacturer: {1}\r\n" +
                "Device Firmware Version: {2}\r\n" +
                "Device Hardware Version: {3}\r\n" +
                "Application Version: {4}\r\n" +
                "---------------------------------",
                DeviceStatus.DeviceName,
                DeviceStatus.DeviceManufacturer,
                DeviceStatus.DeviceFirmwareVersion,
                DeviceStatus.DeviceHardwareVersion,
                version);

            // Email task
            var email = new EmailComposeTask();
            email.To = "mrlordkaj@gmail.com";
            email.Subject = "Pyramid Raider Customer Feedback";
            email.Body = body;

            email.Show();
        }
#endif
    }
}
