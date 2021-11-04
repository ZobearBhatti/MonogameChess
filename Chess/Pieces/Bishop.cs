using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Bishop : Piece
    {
        public Bishop(Texture2D texture, byte colour) : base(texture, colour)
        {
            Type = 2;
        }

        protected override void OnGenerateLegalMoves(int xpos, int ypos, bool _attackOnly)
        {
            int i = 1;  // diagonally up left
            while (i < 8) 
            {
                if (xpos - i < 0 || ypos - i < 0) // if out of bounds
                    break;

                if (_board[xpos - i, ypos - i] is Piece)    // if hit piece
                {
                    if (_board[xpos - i, ypos - i].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos - i, ypos - i));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos - i, ypos - i));   // add position
                i++;    // increment i
            }

            i = 1;  // diagonally up right
            while (i < 8)
            {
                if (xpos + i > 7 || ypos - i < 0) // if out of bounds
                    break;

                if (_board[xpos + i, ypos - i] is Piece)    // if hit piece
                {
                    if (_board[xpos + i, ypos - i].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos + i, ypos - i));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos + i, ypos - i));   // add position
                i++;    // increment i
            }

            i = 1;  // diagonally down left
            while (i < 8)
            {
                if (xpos - i < 0 || ypos + i > 7) // if out of bounds
                    break;

                if (_board[xpos - i, ypos + i] is Piece)    // if hit piece
                {
                    if (_board[xpos - i, ypos + i].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos - i, ypos + i));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos - i, ypos + i));   // add position
                i++;    // increment i
            }

            i = 1;  // diagonally down right
            while (i < 8)
            {
                if (xpos + i > 7 || ypos + i > 7) // if out of bounds
                    break;

                if (_board[xpos + i, ypos + i] is Piece)    // if hit piece
                {
                    if (_board[xpos + i, ypos + i].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos + i, ypos + i));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos + i, ypos + i));   // add position
                i++;    // increment i
            }
        }
    }
}
