﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Queen : Piece
    {
        public Queen(Texture2D texture, byte colour) : base(texture, colour)
        {
            Type = 1;
        }
        protected override void OnGenerateLegalMoves(int xpos, int ypos)
        {
            int i = 1;  // up
            while (i < 8)
            {
                if (ypos - i < 0) // if out of bounds
                    break;

                if (_board[xpos, ypos - i] is Piece)    // if hit piece
                {
                    if (_board[xpos, ypos - i].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos, ypos - i));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos, ypos - i));   // add position
                i++;    // increment i
            }

            i = 1;  // down
            while (i < 8)
            {
                if (ypos + i > 7) // if out of bounds
                    break;

                if (_board[xpos, ypos + i] is Piece)    // if hit piece
                {
                    if (_board[xpos, ypos + i].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos, ypos + i));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos, ypos + i));   // add position
                i++;    // increment i
            }

            i = 1;  // left
            while (i < 8)
            {
                if (xpos - i < 0) // if out of bounds
                    break;

                if (_board[xpos - i, ypos] is Piece)    // if hit piece
                {
                    if (_board[xpos - i, ypos].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos - i, ypos));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos - i, ypos));   // add position
                i++;    // increment i
            }

            i = 1;  // right
            while (i < 8)
            {
                if (xpos + i > 7) // if out of bounds
                    break;

                if (_board[xpos + i, ypos] is Piece)    // if hit piece
                {
                    if (_board[xpos + i, ypos].Colour != base.Colour) // if hit OTHER colour
                    {
                        _legalMoves.Add(new Vector2(xpos + i, ypos));   // add position
                    }
                    break;  //break lol
                }

                _legalMoves.Add(new Vector2(xpos + i, ypos));   // add position
                i++;    // increment i
            }

            i = 1;  // diagonally up left
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
