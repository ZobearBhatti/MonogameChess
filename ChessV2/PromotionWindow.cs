using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class PromotionWindow
    {
        private Texture2D _texture;
        private Texture2D _bgTexture;
        private int _colour;
        private int _choice;

        private MouseState _prevMouseState;

        public int Choice { get { return _choice; } }

        public PromotionWindow(Texture2D texture, int colour, Texture2D bgTexture)
        {
            _texture = texture; _colour = 1 - colour;
            _choice = 4; // choice SHOULD be 0-3 so 4 by default cuz int aint f*ing nullable
            _bgTexture = bgTexture;
            _prevMouseState = new MouseState();
        }


        public void Update(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                int x = mouseState.X;
                int y = mouseState.Y;

                if (x > 200 && x < 600 && y > 350 && y < 450)
                {
                    _choice = (x - 200) / 100;
                }
            }
            _prevMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bgTexture, new Rectangle(200, 350, 400, 100), Color.LightBlue);

            spriteBatch.Draw(_texture, new Rectangle(400, 400, 400, 100),
                new Rectangle(200, 200 * _colour, 800, 200), Color.White, 
                0f, new Vector2(400, 100), SpriteEffects.None, 1f);
        }
    }
}
