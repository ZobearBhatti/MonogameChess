using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(int colour) : base(colour)
        {
            base.Type = 2;
        }

        public override void GenerateLegalMoves(Square[,] Board)
        {
            LegalMoves.Clear();
            int X = base.File; int Y = base.Rank;

            // up left
            for (int i = 1; i <= Math.Min(X, Y); i++) 
            {
                if (Board[X - i, Y - i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X - i, Y - i, Board, 0, false)); }
                else if (Board[X - i, Y - i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X - i, Y - i, Board, 0, false)); break; }
                else { break; }
            }

            // up right
            for (int i = 1; i <= Math.Min(7 - X, Y); i++)
            {
                if (Board[X + i, Y - i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X + i, Y - i, Board, 0, false)); }
                else if (Board[X + i, Y - i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X + i, Y - i, Board, 0, false)); break; }
                else { break; }
            }

            // down left
            for (int i = 1; i <= Math.Min(X, 7 - Y); i++)
            {
                if (Board[X - i, Y + i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X - i, Y + i, Board, 0, false)); }
                else if (Board[X - i, Y + i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X - i, Y + i, Board, 0, false)); break; }
                else { break; }
            }

            // down right
            for (int i = 1; i <= Math.Min(7 - X, 7 - Y); i++)
            {
                if (Board[X + i, Y + i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X + i, Y + i, Board, 0, false)); }
                else if (Board[X + i, Y + i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X + i, Y + i, Board, 0, false)); break; }
                else { break; }
            }

            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }
    }
}
