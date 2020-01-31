using System;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using OpenitvnGame.Helper;

namespace OpenitvnGame
{
    class SoundController : IGameComponent
    {
        public const string SETTING_FILE = "setting";
        const string RECORD_MUSIC = "setting_music";
        const string RECORD_SOUND = "setting_sound";

        enum MusicState { FadeIn, FadeOut, FadeOutHalf, Pause }

        MusicState state = MusicState.FadeIn;
        Song music;

        public bool AllowChangeMusicSetting { get; private set; } //cho phep chinh setting nhac nen
        public bool DummyMusicSetting { get; private set; } //setting nhac nen la gia
        public bool IsSound { get; private set; }
        public bool IsMusic { get; private set; }

        static SoundController _instance;
        public static SoundController CreateInstance(Game parent) { return new SoundController(parent); }
        public static SoundController GetInstance() { return _instance; }
        Game main;

        Texture2D texLock;
        Sprite2D sprMusic, sprSound;

        public SoundController(Game parent)
        {
            if (_instance != null) throw new Exception("Only one Music instance can be created.");
            else _instance = this;
            main = parent;
        }

#if WINDOWS_PHONE
        public void Initialize()
        {
            //doc setting da luu
            IsMusic = StorageHelper.GetSetting<bool>(RECORD_MUSIC, true);
            IsSound = StorageHelper.GetSetting<bool>(RECORD_SOUND, true);
            //IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            //IsolatedStorageFileStream stream = store.OpenFile(SETTING_FILE, FileMode.OpenOrCreate, FileAccess.Read);
            //StreamReader reader = new StreamReader(stream);
            //try
            //{
            //    IsMusic = reader.ReadLine() != "0";
            //    IsSound = reader.ReadLine() != "0";
            //}
            //catch
            //{
            //    IsMusic = true;
            //    IsSound = true;
            //}
            //finally
            //{
            //    reader.Close();
            //    stream.Close();
            //}

            DummyMusicSetting = IsMusic;

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

            ContentManager content = main.Content;
            Texture2D texMusic = content.Load<Texture2D>("Images/btnMusic");
            sprMusic = new Sprite2D(texMusic, texMusic.Width, texMusic.Height / 2);
            Texture2D texSound = content.Load<Texture2D>("Images/btnSound");
            sprSound = new Sprite2D(texSound, texSound.Width, texSound.Height / 2);
            texLock = content.Load<Texture2D>("Images/lockMusic");
            updateButton();

            MediaPlayer.IsRepeating = true;
        }
#endif
#if WINDOWS
        public void Initialize()
        {
            IsMusic = true;
            IsSound = true;
            AllowChangeMusicSetting = true;
            ContentManager content = main.Content;
            Texture2D texMusic = content.Load<Texture2D>("Images/btnMusic");
            sprMusic = new Sprite2D(texMusic, texMusic.Width, texMusic.Height / 2);
            Texture2D texSound = content.Load<Texture2D>("Images/btnSound");
            sprSound = new Sprite2D(texSound, texSound.Width, texSound.Height / 2);
            texLock = content.Load<Texture2D>("Images/lockMusic");
            updateButton();
        }
#endif

        public void Update()
        {
            if (music == null) return;

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

        public void SetBackgroundMusic(Song song)
        {
            if (!song.Equals(music))
            {
                music = song;
                if (IsMusic)
                {
                    MediaPlayer.Volume = 0;
                    try
                    {
                        MediaPlayer.Play(music);
                    }
                    catch { }
                    MediaPlayer.Pause();
                    state = MusicState.Pause;
                }
            }
        }

        public void Play()
        {
            if (!IsMusic || music == null) return;
            MediaPlayer.Resume();
            MediaPlayer.Volume = 1;
        }

        public void FadeIn()
        {
            if (!IsMusic || music == null) return;
            MediaPlayer.Resume();
            state = MusicState.FadeIn;
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

        private void updateButton()
        {
            sprMusic.SetFrame((IsMusic || !AllowChangeMusicSetting) ? 1 : 0);
            sprSound.SetFrame(IsSound ? 1 : 0);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle desMusic, Rectangle desSound)
        {
            sprMusic.SetPosition(desMusic.X, desMusic.Y);
            sprMusic.Draw(spriteBatch);
            sprSound.SetPosition(desSound.X, desSound.Y);
            sprSound.Draw(spriteBatch);
            if (!AllowChangeMusicSetting)
            {
                Vector2 vtLock = new Vector2(
                    desMusic.X - (texLock.Width - desMusic.Width) / 2,
                    desMusic.Y - (texLock.Height - desMusic.Height) / 2
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
                        MediaPlayer.Play(music);
                    }
                    catch { }
                    MediaPlayer.Volume = 0;
                    state = MusicState.FadeIn;
                }
                else
                {
                    state = MusicState.FadeOut;
                }
                updateButton();
            }
        }

        public void ToggleSound()
        {
            IsSound = !IsSound;
            updateButton();
        }

        public static void PlaySound(SoundEffect sound)
        {
            if (_instance.IsSound) sound.Play();
        }

        //public static void SaveSetting()
        //{
        //    IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        //    FileStream stream = store.OpenFile(SETTING_FILE, FileMode.Create, FileAccess.Write);
        //    StreamWriter writer = new StreamWriter(stream);
        //    try
        //    {
        //        if (_instance.AllowChangeMusicSetting) writer.WriteLine(_instance.IsMusic ? 1 : 0);
        //        else writer.WriteLine(_instance.DummyMusicSetting ? 1 : 0);
        //        writer.WriteLine(_instance.IsSound ? 1 : 0);
        //    }
        //    catch { }
        //    finally
        //    {
        //        writer.Close();
        //        stream.Close();
        //    }
        //}
    }
}
