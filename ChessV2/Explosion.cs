using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class Explosion
    {
        private Texture2D _texture;
        private int _timer;
        private Rectangle rectangle;

        public bool IsDisposed { get; set; }

        public Explosion(Texture2D texture, int X, int Y)
        {
            _texture = texture; rectangle = new Rectangle(X * 100, Y * 100, 100, 100);
            _timer = 0;
        }

        public void Update()
        {
            if (_timer > 12)
            {
                IsDisposed = true;
            }
            _timer++;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, rectangle, new Rectangle(_timer * 50, 0, 50, 50), Color.White);
        }

    }
}
