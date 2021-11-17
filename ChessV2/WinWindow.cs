using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class WinWindow
    {
        private Texture2D _texture;
        private SpriteFont _font;
        private string _message;
        private float _position;
        private float _timer;

        public WinWindow(string message, Texture2D texture, SpriteFont font)
        {
            _texture = texture; _message = message; _font = font;
            _position = 0f; _timer = -3f;
        }

        public void Update(GameTime gameTime)
        {
            if (_timer < 0)
            {
                _timer += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 100);
            }
            else if (_position < 400)
            {
                _timer += (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                _position = 400 * Ease(_timer); 
            }
            else
            {
                _position = 400f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_timer >= 0)
            {
                spriteBatch.Draw(_texture, new Vector2(400, _position), null, Color.White, 0f,
                    new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, 1f);
            }
        }

        private float Ease(float t) // ease Out Expo function => (tween)
        {
            return (t == 1) ? 1f : (float)(1 - Math.Pow(2, -10 * t));
        }
    }
}
