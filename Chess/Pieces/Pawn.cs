using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Pawn : Piece
    {
        public Pawn(Texture2D texture, byte colour) : base(texture, colour)
        {
            Type = 5;
        }
        protected override void OnGenerateLegalMoves(int xpos, int ypos, bool _attackOnly)
        {
            switch (base.Colour)
            {
                case 0:
                    genWhite(xpos, ypos, _attackOnly); break;
                case 1:
                    genBlack(xpos, ypos, _attackOnly); break;
            }
        }

        private void genWhite(int xpos, int ypos, bool _attackOnly)
        {
            if (!_attackOnly)
            {
                // generate forward movement
                {
                    if (ypos == 6) // if not moved (able to double move)
                    {
                        for (int i = 1; i <= 2; i++)    // loop to check square above and square above THAT
                        {
                            if (_board[xpos, ypos - i] is Piece) // if there is a piece above
                            { break; } // neither move valid, break loop
                            _legalMoves.Add(new Vector2(xpos, ypos - i)); // if squre unoccupied, add move to list
                        }
                    }
                    else // if has moved (unable to double move)
                    {
                        if (!(_board[xpos, ypos - 1] is Piece)) // if square above NOT occupied
                        { _legalMoves.Add(new Vector2(xpos, ypos - 1)); }   // is legal move
                    }
                }

                // generate taking moves
                {
                    // take up left
                    if (xpos > 0)   // if not on A file
                    {
                        if (_board[xpos - 1, ypos - 1] is Piece)    // if square above left is piece
                        {
                            if (_board[xpos - 1, ypos - 1].Colour != base.Colour)   // if said piece is opposite colour
                                _legalMoves.Add(new Vector2(xpos - 1, ypos - 1));   // legal move
                        }
                        else if (_board[xpos - 1, ypos] is Pawn && _board[xpos - 1, ypos].Colour != base.Colour)
                        // if pawn to left:
                        {
                            if (_board[xpos - 1, ypos]._canBeEnPassant) // if pawn can be enpassanted
                            {
                                _legalMoves.Add(new Vector2(xpos - 1, ypos - 1));   // legal move
                            }
                        }
                    }

                    // take up right
                    if (xpos < 7)   // if not on H file
                    {
                        if (_board[xpos + 1, ypos - 1] is Piece)    // if square above right is piece
                        {
                            if (_board[xpos + 1, ypos - 1].Colour != base.Colour)   // if said piece is opposite colour
                                _legalMoves.Add(new Vector2(xpos + 1, ypos - 1));   // legal move
                        }
                        else if (_board[xpos + 1, ypos] is Pawn && _board[xpos + 1, ypos].Colour != base.Colour) // if pawn to right:
                        {
                            if (_board[xpos + 1, ypos]._canBeEnPassant) // if pawn can be enpassanted
                            {
                                _legalMoves.Add(new Vector2(xpos + 1, ypos - 1));   // legal move
                            }
                        }
                    }
                }
            } // normal code

            else
            {
                if (xpos > 0) // gen left take
                { _legalMoves.Add(new Vector2(xpos - 1, ypos - 1)); }
                if (xpos < 7) // gen right take
                { _legalMoves.Add(new Vector2(xpos + 1, ypos - 1)); }
            }
        }

        private void genBlack(int xpos, int ypos, bool _attackOnly)
        {
            if (!_attackOnly) // normal code
            {
                // generate forward movement
                {
                    if (ypos == 1) // if not moved (able to double move)
                    {
                        for (int i = 1; i <= 2; i++)    // loop to check square above and square above THAT
                        {
                            if (_board[xpos, ypos + i] is Piece) // if there is a piece above
                            { break; } // neither move valid, break loop
                            _legalMoves.Add(new Vector2(xpos, ypos + i)); // if squre unoccupied, add move to list
                        }
                    }
                    else // if has moved (unable to double move)
                    {
                        if (!(_board[xpos, ypos + 1] is Piece)) // if square above NOT occupied
                        { _legalMoves.Add(new Vector2(xpos, ypos + 1)); }   // is legal move
                    }
                }

                // generate taking moves
                {
                    // take below left
                    if (xpos > 0)   // if not on A file
                    {
                        if (_board[xpos - 1, ypos + 1] is Piece)    // if square below left is piece
                        {
                            if (_board[xpos - 1, ypos + 1].Colour != base.Colour)   // if said piece is opposite colour
                                _legalMoves.Add(new Vector2(xpos - 1, ypos + 1));   // legal move
                        }
                        else if (_board[xpos - 1, ypos] is Pawn && _board[xpos - 1, ypos].Colour != base.Colour)
                        // if pawn to left:
                        {
                            if (_board[xpos - 1, ypos]._canBeEnPassant) // if pawn can be enpassanted
                            {
                                _legalMoves.Add(new Vector2(xpos - 1, ypos + 1));   // legal move
                            }
                        }
                    }

                    // take below right
                    if (xpos < 7)   // if not on H file
                    {
                        if (_board[xpos + 1, ypos + 1] is Piece)    // if square above right is piece
                        {
                            if (_board[xpos + 1, ypos + 1].Colour != base.Colour)   // if said piece is opposite colour
                                _legalMoves.Add(new Vector2(xpos + 1, ypos + 1));   // legal move
                        }
                        else if (_board[xpos + 1, ypos] is Pawn && _board[xpos + 1, ypos].Colour != base.Colour) // if pawn to right:
                        {
                            if (_board[xpos + 1, ypos]._canBeEnPassant) // if pawn can be enpassanted
                            {
                                _legalMoves.Add(new Vector2(xpos + 1, ypos + 1));   // legal move
                            }
                        }
                    }
                }
            }

            else
            {
                if (xpos > 0) // gen left take
                { _legalMoves.Add(new Vector2(xpos - 1, ypos + 1)); }
                if (xpos < 7) // gen right take
                { _legalMoves.Add(new Vector2(xpos + 1, ypos + 1)); }
            }
        }
    }
}
