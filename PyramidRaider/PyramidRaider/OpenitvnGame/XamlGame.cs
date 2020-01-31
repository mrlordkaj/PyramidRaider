#if WP7
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows;

//re-definition Microsoft.Xna.Framework.Game for using Silverlight
namespace Microsoft.Xna.Framework
{
    public class Game
    {
        public ContentManager Content { get; set; }

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (_graphicsDevice == null) _graphicsDevice = SharedGraphicsDeviceManager.Current.GraphicsDevice;
                return _graphicsDevice;
            }
        }

        protected virtual void Initialize() { }
        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }
        protected virtual void Update(GameTime gameTime) { }
        protected virtual void Draw(GameTime gameTime) { }

        //caller
        public void CInitialize() { Initialize(); }
        public void CLoadContent() { LoadContent(); }
        public void CUpdate(GameTime gameTime) { Update(gameTime); }
        public void CDraw(GameTime gameTime) { Draw(gameTime); }
    }
}
#endif