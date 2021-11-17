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
            base.Type = 0; CanCastle = true;
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
                    if (Board[X + Xo, Y + Yo].ContainsPiece())
                    {
                        if (Board[X + Xo, Y + Yo].Piece.Colour == base.Colour)
                        {
                            continue;
                        }
                    }
                    LegalMoves.Add(new Move(X, Y, X + Xo, Y + Yo, Board, 0, false));
                }
                else { }
            }

            // CHECK FOR CASTLING
            if (CanCastle)
            {
                GenCastleMoves(Board, Colour);
            }


            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }

        private void GenCastleMoves(Square[,] Board, int colour)
        {
            for (int i = File + 1; i < 8; i++) // right
            {
                // check no space between king and castle
                if (Board[i, Rank].ContainsPiece() && i != 7) // if piece between king/rook, break
                {
                    break;
                }
                else if (i == 7 && Board[i, Rank].Piece is Rook && Board[i, Rank].Piece.Colour == colour && Board[Rank, i].Piece.CanCastle)
                {
                    // VERIFY NO SQUARES ARE IN CHECK
                    for (int x = File + 1; x < 7; x++)
                    {
                        // add king
                        Board[x, Rank].AddPiece(new King(base.Colour));

                        // see if its in check
                        if ((Board[x, Rank].Piece as King).IsInCheck(Board, Board[x, Rank], Board[File, Rank]))
                        {
                            Board[x, Rank].RemovePiece(); break;
                        }
                        
                        else if (x == 6)
                        {
                            Board[x, Rank].RemovePiece();
                            LegalMoves.Add(new Move(File, Rank, 6, Rank, Board, 0, true));
                        }
                    }
                }
            }
            for (int i = File - 1; i >= 0; i--) // right
            {
                // check no space between king and castle
                if (Board[i, Rank].ContainsPiece() && i != 0) // if piece between king/rook, break
                {
                    break;
                }
                else if (i == 0 && Board[i, Rank].Piece is Rook && Board[i, Rank].Piece.Colour == colour && Board[Rank, i].Piece.CanCastle)
                {
                    LegalMoves.Add(new Move(File, Rank, 2, Rank, Board, 0, true));
                }
            }
        }

        public bool IsInCheck(Square[,] Board, Square KingSquare, Square SquareFrom)
        {
            int X = KingSquare.File; int Y = KingSquare.Rank;
            try
            {
                // QUEEN / ROOK MOVEMENT
                {
                    if (X > 0)
                    {
                        for (int i = X - 1; i >= 0; i--) // left
                        {
                            if (Board[i, Y].ContainsPiece()) // if square has piece
                            {
                                if (Board[i, Y].Piece.Colour != base.Colour
                                    && (Board[i, Y].Piece is Queen || Board[i, Y].Piece is Rook)) // if its an opp
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                else { break; }
                            }
                        }
                    }

                    if (X < 7)
                    {
                        for (int i = X + 1; i < 8; i++) // right
                        {
                            if (Board[i, Y].ContainsPiece()) // if square has piece
                            {
                                if (Board[i, Y].Piece.Colour != base.Colour
                                    && (Board[i, Y].Piece is Queen || Board[i, Y].Piece is Rook)) // if its an opp
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                else { break; }
                            }
                        }
                    }

                    if (Y > 0)
                    {
                        for (int i = Y - 1; i >= 0; i--) // up
                        {
                            if (Board[X, i].ContainsPiece()) // if square has piece
                            {
                                if (Board[X, i].Piece.Colour != base.Colour
                                    && (Board[X, i].Piece is Queen || Board[X, i].Piece is Rook)) // if its an opp
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                else { break; }
                            }
                        }
                    }

                    if (Y < 7)
                    {
                        for (int i = Y + 1; i < 8; i++) // down
                        {
                            if (Board[X, i].ContainsPiece()) // if square has piece
                            {
                                if (Board[X, i].Piece.Colour != base.Colour
                                    && (Board[X, i].Piece is Queen || Board[X, i].Piece is Rook)) // if its an opp
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                else { break; }
                            }
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
                            {
                                CanCastle = false;
                                return true;
                            }
                        }
                        if (Board[X + 1, Y - 1].Piece is Pawn)  // right up
                        {
                            if (Board[X + 1, Y - 1].Piece.Colour != base.Colour)
                            {
                                CanCastle = false;
                                return true;
                            }
                        }
                        break;

                    case 1: // white is opps
                        if (Board[X - 1, Y + 1].Piece is Pawn)  // left down
                        {
                            if (Board[X - 1, Y + 1].Piece.Colour != base.Colour)
                            {
                                CanCastle = false;
                                return true;
                            }
                        }
                        if (Board[X + 1, Y + 1].Piece is Pawn)  // right down
                        {
                            if (Board[X + 1, Y + 1].Piece.Colour != base.Colour)
                            {
                                CanCastle = false;
                                return true;
                            }
                        }
                        break;
                }

                // QUEEN / BISHOP MOVEMENT
                {
                    // up left
                    if (X > 0 && Y > 0)
                    {
                        for (int i = 1; i <= Math.Min(X, Y); i++)
                        {
                            if (Board[X - i, Y - i].ContainsPiece())
                            {
                                if (Board[X - i, Y - i].Piece.Colour != base.Colour
                                    && (Board[X - i, Y - i].Piece is Bishop || Board[X - i, Y - i].Piece is Queen))
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                break;
                            }
                        }
                    }

                    if (X < 7 && Y > 0)
                    {
                        // up right
                        for (int i = 1; i <= Math.Min(7 - X, Y); i++)
                        {
                            if (Board[X + i, Y - i].ContainsPiece())
                            {
                                if (Board[X + i, Y - i].Piece.Colour != base.Colour
                                    && (Board[X + i, Y - i].Piece is Bishop || Board[X + i, Y - i].Piece is Queen))
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                break;
                            }
                        }
                    }

                    if (X > 0 && Y < 7)
                    {
                        // down left
                        for (int i = 1; i <= Math.Min(X, 7 - Y); i++)
                        {
                            if (Board[X - i, Y + i].ContainsPiece())
                            {
                                if (Board[X - i, Y + i].Piece.Colour != base.Colour
                                    && (Board[X - i, Y + i].Piece is Bishop || Board[X - i, Y + i].Piece is Queen))
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                break;
                            }
                        }
                    }

                    if (X < 7 && Y < 7)
                    {
                        // down right
                        for (int i = 1; i <= Math.Min(7 - X, 7 - Y); i++)
                        {
                            if (Board[X + i, Y + i].ContainsPiece())
                            {
                                if (Board[X + i, Y + i].Piece.Colour != base.Colour
                                    && (Board[X + i, Y + i].Piece is Bishop || Board[X + i, Y + i].Piece is Queen))
                                {
                                    CanCastle = false;
                                    return true;
                                }
                                break;
                            }
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
                            if (Board[X + Xo, Y + Yo].ContainsPiece())
                            {
                                if (Board[X + Xo, Y + Yo].Piece.Colour != base.Colour
                                    && Board[X + Xo, Y + Yo].Piece is Knight)
                                {
                                    CanCastle = false;
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
                            if (Board[X + Xo, Y + Yo].ContainsPiece())
                            {
                                if (Board[X + Xo, Y + Yo].Piece.Colour != base.Colour
                                    && Board[X + Xo, Y + Yo].Piece is King)
                                {
                                    CanCastle = false;
                                    return true;
                                }
                            }
                        }
                        else { }
                    }
                }
            }
            catch { }

            return false;
        }
    }
}
