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

        public int x { get; set; }
        public int y { get; set; }

        public bool _canBeEnPassant;
        public bool _canCastle;

        public byte Colour { get { return _colour; } set { _colour = value; } }
        public byte Type { get { return _type; } set { _type = value; } }

        public Piece(Texture2D texture, byte colour)
        {
            _texture = texture; _colour = colour;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(_texture, new Rectangle(x * 100, y * 100, 100, 100),
                new Rectangle(_type * 200, _colour * 200, 200, 200), Color.White);
        }

        public List<Vector2> GenerateLegalMoves(Piece[,] Board, bool _attackOnly)
        {
            _board = Board;
            _legalMoves.Clear(); // clear list of legal moves
            OnGenerateLegalMoves(x, y, _attackOnly); // generate all legal moves
            return _legalMoves;
        }

        protected virtual void OnGenerateLegalMoves(int xpos, int ypos, bool _attackOnly)
        {
            
        }

        protected bool MoveResultsInCheck(Vector2 move, int xfrom, int yfrom, Piece[,] boardState)
        {
            return false;
        }
    }
}
