using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Pawn : Piece
    {
        bool _canEnPassant = false;
        bool _canBeEnPassant = false;
        public int PrevYPos { get; set; }

        public Pawn(Texture2D texture, byte colour) : base(texture, colour)
        {
            Type = 5;
        }
        protected override void OnGenerateLegalMoves(int xpos, int ypos)
        {
            if (base.Colour == 0) // WHITE
            {
                while (true) // GENERATE NORMAL MOVES
                {
                    if (_board[xpos, ypos - 1] is Piece) // single move
                    { break; } else { _legalMoves.Add(new Vector2(xpos, ypos - 1)); }

                    if (ypos == 6)
                    {
                        if (!(_board[xpos, ypos - 2] is Piece)) // double move
                        { _legalMoves.Add(new Vector2(xpos, ypos - 2)); }
                    }
                    break;
                }
                if (_board[Math.Min(xpos + 1,7), ypos - 1] is Piece && xpos < 7)    // take piece above right
                {
                    if (_board[Math.Min(xpos + 1, 7), ypos - 1].Colour != base.Colour)
                    { _legalMoves.Add(new Vector2(xpos + 1, ypos - 1)); }
                }
                if (_board[Math.Max(xpos - 1, 0), ypos - 1] is Piece && xpos > 0)    // take piece above left
                {
                    if (_board[Math.Max(xpos - 1, 0), ypos - 1].Colour != base.Colour)
                    { _legalMoves.Add(new Vector2(xpos - 1, ypos - 1)); }
                }
                if (xpos > 0 && ypos == 3) // check en passant left
                {
                    if (_board[xpos - 1, ypos] is Pawn && !(_board[xpos-1, ypos-1] is Piece)) // if piece to left is pawn and swuare above is free
                    {
                        if (_board[xpos - 1, ypos].PrevYPos == 1) // if piece can be enpassanted
                        {
                            _legalMoves.Add(new Vector2(xpos - 1, ypos - 1));
                        }
                    }
                }
            }
            if (base.Colour == 1) // BLACK
            {
                while (true) // GENERATE NORMAL MOVES
                {
                    if (_board[xpos, ypos + 1] is Piece) // single move
                    { break; }
                    else { _legalMoves.Add(new Vector2(xpos, ypos + 1)); }

                    if (ypos == 1)
                    {
                        if (!(_board[xpos, ypos + 2] is Piece)) // double move
                        { _legalMoves.Add(new Vector2(xpos, ypos + 2)); }
                    }
                    break;
                }
                if (_board[Math.Min(xpos + 1, 7), ypos + 1] is Piece && xpos < 7)    // take piece below right
                {
                    if (_board[Math.Min(xpos + 1, 7), ypos + 1].Colour != base.Colour)
                    { _legalMoves.Add(new Vector2(xpos + 1, ypos + 1)); }
                }
                if (_board[Math.Max(xpos - 1,0), ypos + 1] is Piece && xpos > 0)    // take piece left
                {
                    if (_board[Math.Max(xpos - 1,0), ypos + 1].Colour != base.Colour)
                    { _legalMoves.Add(new Vector2(xpos - 1, ypos + 1)); }
                }
            }
        }
    }
}
