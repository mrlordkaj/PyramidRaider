using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using OpenitvnGame.Helpers;

namespace OpenitvnGame
{
    class SoundController
    {
        public const string SETTING_FILE = "setting";
        const string RECORD_MUSIC = "setting_music";
        const string RECORD_SOUND = "setting_sound";
        const string TEXTURE_LOCK = "Images/lockMusic";
        const string TEXTURE_DENY = "Images/cross";

        enum MusicState { FadeIn, FadeOut, FadeOutHalf, Pause }

        MusicState state = MusicState.FadeIn;
        Song music;

        public bool AllowChangeMusicSetting { get; private set; } //cho phep chinh setting nhac nen
        public bool AllowChangeSoundSetting { get; private set; } //cho phep chinh setting am thanh
        public bool IsSound { get; private set; }
        public bool IsMusic { get; private set; }

        static SoundController _instance;
        public static SoundController CreateInstance(Game parent) { return new SoundController(parent); }
        public static SoundController GetInstance() { return _instance; }
        Game main;

        Texture2D texLock, texCross;

        public SoundController(Game parent)
        {
            _instance = this;
            main = parent;
        }

        public void LoadContent()
        {
            try
            {
#if WP7
                FrameworkDispatcher.Update();
#endif
                //doc setting da luu
                IsMusic = SettingHelper.GetSetting<bool>(RECORD_MUSIC, true);
                IsSound = SettingHelper.GetSetting<bool>(RECORD_SOUND, true);

                //neu he thong dang phat nhac truoc khi mo app thi khoa chuc nang dieu khien nhac nen
                if (MediaPlayer.State == MediaState.Playing)
                {
                    IsMusic = false;
                    AllowChangeMusicSetting = false;
                }
                else
                {
                    AllowChangeMusicSetting = true;
                }
                AllowChangeSoundSetting = true;

                MediaPlayer.IsRepeating = true;
            }
            catch
            {
                IsMusic = AllowChangeMusicSetting = false;
                IsSound = AllowChangeSoundSetting = false;
            }

            texLock = main.Content.Load<Texture2D>(TEXTURE_LOCK);
            texCross = main.Content.Load<Texture2D>(TEXTURE_DENY);
        }

        public void Update()
        {
            if (music == null) return;

            try
            {
#if WP7
                FrameworkDispatcher.Update();
#endif
                switch (state)
                {
                    case MusicState.FadeIn:
                        if (MediaPlayer.Volume < 1) MediaPlayer.Volume += 0.01f;
                        break;

                    case MusicState.FadeOutHalf:
                        if (MediaPlayer.Volume > 0.1f) MediaPlayer.Volume -= 0.02f;
                        else state = MusicState.Pause;
                        break;

                    case MusicState.FadeOut:
                        if (MediaPlayer.Volume > 0) MediaPlayer.Volume -= 0.02f;
                        else
                        {
                            MediaPlayer.Pause();
                            state = MusicState.Pause;
                        }
                        break;

                }
            }
            catch { }
        }

        public void SetBackgroundMusic(Song song)
        {
            if (!song.Equals(music))
            {
                music = song;
                if (IsMusic)
                {
                    try
                    {
#if WP7
                        FrameworkDispatcher.Update();
#endif
                        MediaPlayer.Volume = 0;
                        MediaPlayer.Play(music);
                        MediaPlayer.Pause();
                        state = MusicState.Pause;
                    }
                    catch { }
                }
            }
        }

        public void Play()
        {
            if (!IsMusic || music == null) return;

            try
            {
#if WP7
                FrameworkDispatcher.Update();
#endif
                MediaPlayer.Resume();
                MediaPlayer.Volume = 1;
            }
            catch { }
        }

        public void FadeIn()
        {
            if (!IsMusic || music == null) return;

            try
            {
#if WP7
                FrameworkDispatcher.Update();
#endif
                MediaPlayer.Resume();
                state = MusicState.FadeIn;
            }
            catch { }
        }

        public void FadeOutHalf()
        {
            if (!IsMusic || music == null) return;
            state = MusicState.FadeOutHalf;
        }

        public void FadeOut()
        {
            if (!IsMusic || music == null) return;
            state = MusicState.FadeOut;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle desMusic, Rectangle desSound)
        {
            if (!IsMusic) spriteBatch.Draw(texCross, desMusic, Color.White);
            if (!IsSound) spriteBatch.Draw(texCross, desSound, Color.White);
            Vector2 vtLock;
            if (!AllowChangeMusicSetting)
            {
                vtLock = new Vector2(
                    desMusic.X - (texLock.Width - desMusic.Width) / 2,
                    desMusic.Y - (texLock.Height - desMusic.Height) / 2
                );
                spriteBatch.Draw(texLock, vtLock, Color.White);
            }
            if (!AllowChangeSoundSetting)
            {
                vtLock = new Vector2(
                    desSound.X - (texLock.Width - desSound.Width) / 2,
                    desSound.Y - (texLock.Height - desSound.Height) / 2
                );
                spriteBatch.Draw(texLock, vtLock, Color.White);
            }
        }

        public void ToggleMusic()
        {
            if (AllowChangeMusicSetting)
            {
                IsMusic = !IsMusic;
                if (IsMusic && music != null)
                {
                    try
                    {
#if WP7
                        FrameworkDispatcher.Update();
#endif
                        MediaPlayer.Play(music);
                        MediaPlayer.Volume = 0;
                        state = MusicState.FadeIn;
                    }
                    catch { }
                }
                else
                {
                    state = MusicState.FadeOut;
                }
                SettingHelper.StoreSetting(RECORD_MUSIC, IsMusic, true);
            }
        }

        public void ToggleSound()
        {
            if (AllowChangeSoundSetting)
            {
                IsSound = !IsSound;
                SettingHelper.StoreSetting(RECORD_SOUND, IsSound, true);
            }
        }

        public static void PlaySound(SoundEffect sound)
        {
            if (_instance.IsSound)
            {
                try
                {
                    sound.Play();
                }
                catch { }
            }
        }
    }
}
