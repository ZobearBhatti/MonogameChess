using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class King : Piece
    {
        private int[,] Offsets = new int[2, 8]
        {
            { 0, 1, 1, 1, 0, -1, -1, -1 },
            { 1, 1, 0, -1, -1, -1, 0, 1 },
        };

        public King(int colour) : base(colour)
        {
            base.Type = 0;
        }

        public override void GenerateLegalMoves(Square[,] Board)
        {
            LegalMoves.Clear();
            int X = base.File; int Y = base.Rank;

            for (int i = 0; i < 8; i++)
            {
                int Xo = Offsets[0, i]; int Yo = Offsets[1, i];
                if (X + Xo >= 0 && X + Xo < 8 && Y + Yo >= 0 && Y + Yo < 8)
                {
                    if (Board[X + Xo, Y + Yo].Piece != null)
                    {
                        if (Board[X + Xo, Y + Yo].Piece.Colour == base.Colour)
                        {
                            continue;
                        }
                    }
                    LegalMoves.Add(new Move(X, Y, X + Xo, Y + Yo, Board));
                }
                else { }
            }

            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }

        public bool IsInCheck(Square[,] Board, Square KingSquare, Square SquareFrom)
        {
            int X = KingSquare.File; int Y = KingSquare.Rank;

            // QUEEN / ROOK MOVEMENT
            {
                for (int i = X - 1; i >= 0; i--) // left
                {
                    if (Board[i, Y].Piece != null) // if square has piece
                    {
                        if (Board[i, Y].Piece.Colour != base.Colour
                            && (Board[i, Y].Piece is Queen || Board[i, Y].Piece is Rook)) // if its an opp
                        {
                            return true;
                        }
                        else { break; }
                    }
                }
                for (int i = X + 1; i < 8; i++) // right
                {
                    if (Board[i, Y].Piece != null) // if square has piece
                    {
                        if (Board[i, Y].Piece.Colour != base.Colour
                            && (Board[i, Y].Piece is Queen || Board[i, Y].Piece is Rook)) // if its an opp
                        {
                            return true;
                        }
                        else { break; }
                    }
                }
                for (int i = Y - 1; i >= 0; i--) // up
                {
                    if (Board[X, i].Piece != null) // if square has piece
                    {
                        if (Board[X, i].Piece.Colour != base.Colour
                            && (Board[X, i].Piece is Queen || Board[X, i].Piece is Rook)) // if its an opp
                        {
                            return true;
                        }
                        else { break; }
                    }
                }
                for (int i = Y + 1; i < 8; i++) // down
                {
                    if (Board[X, i].Piece != null) // if square has piece
                    {
                        if (Board[X, i].Piece.Colour != base.Colour
                            && (Board[X, i].Piece is Queen || Board[X, i].Piece is Rook)) // if its an opp
                        {
                            return true;
                        }
                        else { break; }
                    }
                }
            }

            // PAWNS
            switch (base.Colour)
            {
                case 0:  // black is opps
                    if (Board[X - 1, Y - 1].Piece is Pawn)  // left up
                    {
                        if (Board[X - 1, Y - 1].Piece.Colour != base.Colour)
                        { return true; }
                    }
                    if (Board[X + 1, Y - 1].Piece is Pawn)  // right up
                    {
                        if (Board[X + 1, Y - 1].Piece.Colour != base.Colour)
                        { return true; }
                    }
                    break;

                case 1: // white is opps
                    if (Board[X - 1, Y + 1].Piece is Pawn)  // left down
                    {
                        if (Board[X - 1, Y + 1].Piece.Colour != base.Colour)
                        { return true; }
                    }
                    if (Board[X + 1, Y + 1].Piece is Pawn)  // right down
                    {
                        if (Board[X + 1, Y + 1].Piece.Colour != base.Colour)
                        { return true; }
                    }
                    break;
            }

            // QUEEN / BISHOP MOVEMENT
            {
                // up left
                for (int i = 1; i <= Math.Min(X, Y); i++)
                {
                    if (Board[X - i, Y - i].Piece != null)
                    {
                        if (Board[X - i, Y - i].Piece.Colour != base.Colour
                            && (Board[X - i, Y - i].Piece is Bishop || Board[X - i, Y - i].Piece is Queen))
                        {
                            return true;
                        }
                        break;
                    }
                }

                // up right
                for (int i = 1; i <= Math.Min(7 - X, Y); i++)
                {
                    if (Board[X + i, Y - i].Piece != null)
                    {
                        if (Board[X + i, Y - i].Piece.Colour != base.Colour
                            && (Board[X + i, Y - i].Piece is Bishop || Board[X + i, Y - i].Piece is Queen))
                        {
                            return true;
                        }
                        break;
                    }
                }

                // down left
                for (int i = 1; i <= Math.Min(X, 7 - Y); i++)
                {
                    if (Board[X - i, Y + i].Piece != null)
                    {
                        if (Board[X - i, Y + i].Piece.Colour != base.Colour
                            && (Board[X - i, Y + i].Piece is Bishop || Board[X - i, Y + i].Piece is Queen))
                        {
                            return true;
                        }
                        break;
                    }
                }

                // down right
                for (int i = 1; i <= Math.Min(7 - X, 7 - Y); i++)
                {
                    if (Board[X + i, Y + i].Piece != null)
                    {
                        if (Board[X + i, Y + i].Piece.Colour != base.Colour
                            && (Board[X + i, Y + i].Piece is Bishop || Board[X + i, Y + i].Piece is Queen))
                        {
                            return true;
                        }
                        break;
                    }
                }
            }

            // KNIGHT MOVEMENT
            {
                int[,] NOffsets = new int[2, 8]
                {
                    { 1, 2, 2, 1, -1, -2, -2, -1 },
                    { 2, 1, -1, -2, -2, -1, 1, 2 },
                };

                for (int i = 0; i < 8; i++)
                {
                    int Xo = NOffsets[0, i]; int Yo = NOffsets[1, i];
                    if (X + Xo >= 0 && X + Xo < 8 && Y + Yo >= 0 && Y + Yo < 8)
                    {
                        if (Board[X + Xo, Y + Yo].Piece != null)
                        {
                            if (Board[X + Xo, Y + Yo].Piece.Colour != base.Colour
                                && Board[X + Xo, Y + Yo].Piece is Knight)
                            {
                                return true;
                            }
                        }
                    }
                    else { }
                }
            }

            // KING MOVEMENT
            {
                for (int i = 0; i < 8; i++)
                {
                    int Xo = Offsets[0, i]; int Yo = Offsets[1, i];
                    if (X + Xo >= 0 && X + Xo < 8 && Y + Yo >= 0 && Y + Yo < 8)
                    {
                        if (Board[X + Xo, Y + Yo].Piece != null)
                        {
                            if (Board[X + Xo, Y + Yo].Piece.Colour != base.Colour
                                && Board[X + Xo, Y + Yo].Piece is King)
                            {
                                return true;
                            }
                        }
                    }
                    else { }
                }
            }

            return false;
        }
    }
}
