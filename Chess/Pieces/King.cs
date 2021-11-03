using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class King : Piece
    {
        private List<Vector2> Offsets = new List<Vector2>()
        {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
            new Vector2(-1,0),
            new Vector2(-1,1)
        };

        public King(Texture2D texture, byte colour) : base(texture, colour)
        {
            Type = 0;
        }
        protected override void OnGenerateLegalMoves(int xpos, int ypos)
        {
            foreach (Vector2 offset in Offsets) // for each possible move
            {
                try // try
                {
                    if (_board[xpos + (int)offset.X, ypos + (int)offset.Y] is Piece) // if hit piece
                    {
                        if (_board[xpos + (int)offset.X, ypos + (int)offset.Y].Colour != base.Colour)   // if not same colour
                        {
                            _legalMoves.Add(new Vector2(xpos + offset.X, ypos + offset.Y)); // add move
                        }
                    }
                    else
                    {
                        _legalMoves.Add(new Vector2(xpos + offset.X, ypos + offset.Y)); // add move
                    }
                }
                catch // if outta bounds
                {

                }
            }
        }
    }
}
