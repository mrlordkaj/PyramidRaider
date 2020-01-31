using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OpenitvnGame
{
    public enum ResolutionCropMethod { Letterbox, Overflow, Softscale };

    public class Resolution
    {
        static bool settingApplied;

        static ResolutionCropMethod _cropMethod;
        public static ResolutionCropMethod CropMethod
        {
            get
            {
                return _cropMethod;
            }
            set
            {
                _cropMethod = value;
                settingApplied = false;
            }
        }

        static float _designWidth;
        public static float DesignWidth
        {
            get
            {
                return _designWidth;
            }
            set
            {
                _designWidth = value;
                settingApplied = false;
            }
        }

        static float _designHeight;
        public static float DesignHeight
        {
            get
            {
                return _designHeight;
            }
            set
            {
                _designHeight = value;
                settingApplied = false;
            }
        }

        static float _realWidth;
        public static float RealWidth
        {
            get
            {
                return _realWidth;
            }
            set
            {
                _realWidth = value;
                settingApplied = false;
            }
        }

        static float _realHeight;
        public static float RealHeight
        {
            get
            {
                return _realHeight;
            }
            set
            {
                _realHeight = value;
                settingApplied = false;
            }
        }

        public static float FactorWidth { get; private set; }
        public static float FactorHeight { get; private set; }
        public static float HardFactor { get; private set; }

        static Rectangle _viewpot;
        public static Rectangle Viewpot
        {
            get
            {
                if (!settingApplied) throw new Exception("Can't using viewpot until apply new settings.");
                return _viewpot;
            }
        }

#if WP7
        static SharedGraphicsDeviceManager _graphics;
#else
        static GraphicsDeviceManager _graphics;
#endif

        static RenderTarget2D _screen;
        public static RenderTarget2D Screen
        {
            get
            {
                if (!settingApplied) throw new Exception("Can't using screen until apply new settings.");
                return _screen;
            }
        }

#if WP7
        public static void Init(SharedGraphicsDeviceManager graphics)
#else
        public static void Init(GraphicsDeviceManager graphics)
#endif
        {
            _graphics = graphics;
            _realWidth = graphics.PreferredBackBufferWidth;
            _realHeight = graphics.PreferredBackBufferHeight;
            settingApplied = false;
        }

        public static void ApplyResolutionSettings()
        {
            if (_graphics == null) throw new Exception("Can't apply settings until init");

            float designAspectRatio = _designWidth / _designHeight;
            float realAspectRatio = _realWidth / _realHeight;
            HardFactor = _realWidth / _designWidth;
            _viewpot = new Rectangle(0, 0, (int)_realWidth, (int)_realHeight);

            switch (_cropMethod)
            {
                case ResolutionCropMethod.Letterbox:
                    if (realAspectRatio > designAspectRatio)
                    {
                        //man hinh thuc rong hon man hinh thiet ke
                        HardFactor = _realHeight / _designHeight;
                        _viewpot.X = (int)((_realWidth - _designWidth * HardFactor) * 0.5f);
                        _viewpot.Y = 0;
                    }
                    else if (realAspectRatio < designAspectRatio)
                    {
                        //man hinh thuc cao hon man hinh thiet ke
                        HardFactor = _realWidth / _designWidth;
                        _viewpot.X = 0;
                        _viewpot.Y = (int)((_realHeight - _designHeight * HardFactor) * 0.5f);
                    }
                    _viewpot.Width = (int)(HardFactor * _designWidth);
                    _viewpot.Height = (int)(HardFactor * _designHeight);
                    break;

                case ResolutionCropMethod.Overflow:
                    if (realAspectRatio > designAspectRatio)
                    {
                        //man hinh thuc rong hon man hinh thiet ke
                        HardFactor = _realWidth / _designWidth;
                        _viewpot.X = 0;
                        _viewpot.Y = (int)((_realHeight - _designHeight * HardFactor) * 0.5f);
                    }
                    else if (realAspectRatio < designAspectRatio)
                    {
                        //man hinh thuc cao hon man hinh thiet ke
                        HardFactor = _realHeight / _designHeight;
                        _viewpot.X = (int)((_realWidth - _designWidth * HardFactor) * 0.5f);
                        _viewpot.Y = 0;
                    }
                    _viewpot.Width = (int)(HardFactor * _designWidth);
                    _viewpot.Height = (int)(HardFactor * _designHeight);
                    break;

                case ResolutionCropMethod.Softscale:
                    FactorWidth = _realWidth / _designWidth;
                    FactorHeight = _realHeight / _designHeight;
                    _viewpot.Width = (int)_realWidth;
                    _viewpot.Height = (int)_realHeight;
                    break;
            }

            _screen = new RenderTarget2D(_graphics.GraphicsDevice, (int)_designWidth, (int)_designHeight);

            settingApplied = true;
        }

        public static void ConvertX(ref int pointerX)
        {
            switch (_cropMethod)
            {
                case ResolutionCropMethod.Letterbox:
                case ResolutionCropMethod.Overflow:
                    pointerX = (int)Math.Ceiling((pointerX - _viewpot.X) / HardFactor);
                    break;

                case ResolutionCropMethod.Softscale:
                    pointerX = (int)Math.Ceiling(pointerX / FactorWidth);
                    break;
            }
        }

        public static void ConvertY(ref int pointerY)
        {
            switch (_cropMethod)
            {
                case ResolutionCropMethod.Letterbox:
                case ResolutionCropMethod.Overflow:
                    pointerY = (int)Math.Ceiling((pointerY - _viewpot.Y) / HardFactor);
                    break;

                case ResolutionCropMethod.Softscale:
                    pointerY = (int)(Math.Ceiling(pointerY / FactorHeight));
                    break;
            }
        }
    }
}
