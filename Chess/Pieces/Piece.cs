using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    public class Piece
    {
        private Texture2D _texture;
        private byte _colour;
        private byte _type;
        protected List<Vector2> _legalMoves = new List<Vector2>();

        protected Piece[,] _board;

        public bool _canBeEnPassant;

        public byte Colour { get { return _colour; } set { _colour = value; } }
        public byte Type { get { return _type; } set { _type = value; } }

        public Piece(Texture2D texture, byte colour)
        {
            _texture = texture; _colour = colour;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch _spriteBatch, int x, int y)
        {
            _spriteBatch.Draw(_texture, new Rectangle(x * 100, y * 100, 100, 100),
                new Rectangle(_type * 200, _colour * 200, 200, 200), Color.White);
        }

        public List<Vector2> GenerateLegalMoves(int x, int y, Piece[,] Board)
        {
            _board = Board;
            _legalMoves.Clear();
            OnGenerateLegalMoves(x, y);
            return _legalMoves;
        }

        protected virtual void OnGenerateLegalMoves(int xpos, int ypos)
        {
            
        }
    }
}
