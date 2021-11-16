using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(int colour) : base(colour)
        {
            base.Type = 5; base.CanBeEnPassant = false;
        }

        public override void GenerateLegalMoves(Square[,] Board)
        {
            LegalMoves.Clear();
            int X = base.File; int Y = base.Rank;
            switch (base.Colour)
            {
                case 0: // white
                    {
                        // generate forward movement
                        for (int i = 1; i <= 2; i++)
                        {
                            if (Board[X, Y - i].ContainsPiece())
                            {
                                break;
                            }
                            else
                            {
                                LegalMoves.Add(new Move(X, Y, X, Y - i, Board));
                            }
                            if (Y != 6) { break; }
                        }

                        // take up left
                        try
                        {
                            if (Board[X - 1, Y - 1].ContainsPiece())
                            {
                                if (Board[X - 1, Y - 1].Piece.Colour != base.Colour)
                                {
                                    LegalMoves.Add(new Move(X, Y, X - 1, Y - 1, Board));
                                }
                            }
                            else if (Board[X - 1, Y].Piece is Pawn) // if pawn to left
                            {
                                if (Board[X - 1, Y].Piece.Colour != base.Colour && 
                                    Board[X - 1, Y].Piece.CanBeEnPassant)    // if pawn is opp and can be enpassanted
                                {
                                    LegalMoves.Add(new Move(X, Y, X - 1, Y - 1, Board, 1));
                                }
                            }
                        } catch { }

                        // take up right
                        try
                        {
                            if (Board[X + 1, Y - 1].ContainsPiece())
                            {
                                if (Board[X + 1, Y - 1].Piece.Colour != base.Colour)
                                {
                                    LegalMoves.Add(new Move(X, Y, X + 1, Y - 1, Board));
                                }
                            }
                            else if (Board[X + 1, Y].Piece is Pawn) // if pawn to right
                            {
                                if (Board[X + 1, Y].Piece.Colour != base.Colour &&
                                    Board[X + 1, Y].Piece.CanBeEnPassant)    // if pawn is opp and can be enpassanted
                                {
                                    LegalMoves.Add(new Move(X, Y, X + 1, Y - 1, Board, 2));
                                }
                            }
                        }
                        catch { }
                        break;
                    }
                case 1: // black
                    {
                        // generate forward movement
                        for (int i = 1; i <= 2; i++)
                        {
                            if (Board[X, Y + i].ContainsPiece())
                            {
                                break;
                            }
                            else
                            {
                                LegalMoves.Add(new Move(X, Y, X, Y + i, Board));
                            }
                            if (Y != 1) { break; }
                        }
                        // take down left
                        try
                        {
                            if (Board[X - 1, Y + 1].ContainsPiece())
                            {
                                if (Board[X - 1, Y + 1].Piece.Colour != base.Colour)
                                {
                                    LegalMoves.Add(new Move(X, Y, X - 1, Y + 1, Board));
                                }
                            }
                            else if (Board[X - 1, Y].Piece is Pawn) // if pawn to left
                            {
                                if (Board[X - 1, Y].Piece.Colour != base.Colour &&
                                    Board[X - 1, Y].Piece.CanBeEnPassant)    // if pawn is opp and can be enpassanted
                                {
                                    LegalMoves.Add(new Move(X, Y, X - 1, Y + 1, Board, 1));
                                }
                            }
                        }
                        catch { }

                        // take down right
                        try
                        {
                            if (Board[X + 1, Y + 1].ContainsPiece())
                            {
                                if (Board[X + 1, Y + 1].Piece.Colour != base.Colour)
                                {
                                    LegalMoves.Add(new Move(X, Y, X + 1, Y + 1, Board));
                                }
                            }
                            else if (Board[X + 1, Y].Piece is Pawn) // if pawn to left
                            {
                                if (Board[X + 1, Y].Piece.Colour != base.Colour &&
                                    Board[X + 1, Y].Piece.CanBeEnPassant)    // if pawn is opp and can be enpassanted
                                {
                                    LegalMoves.Add(new Move(X, Y, X + 1, Y + 1, Board, 2));
                                }
                            }
                        }
                        catch { }
                        break;
                    }

            }

            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }
    }
}
