using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class Queen : Piece
    {
        public Queen(int colour) : base(colour)
        {
            base.Type = 1;
        }

        public override void GenerateLegalMoves(Square[,] Board)
        {
            LegalMoves.Clear();
            int X = base.File; int Y = base.Rank;

            // left
            for (int i = X - 1; i >= 0; i--) // for each piece to the left
            {
                if (Board[i, Y].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, i, Y, Board)); }
                else if (Board[i, Y].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, i, Y, Board)); break; }
                else { break; }
            }

            // right
            for (int i = X + 1; i < 8; i++) // for each piece to the right
            {
                if (Board[i, Y].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, i, Y, Board)); }
                else if (Board[i, Y].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, i, Y, Board)); break; }
                else { break; }
            }

            // up
            for (int i = Y - 1; i >= 0; i--) // for each piece to the right
            {
                if (Board[X, i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X, i, Board)); }
                else if (Board[X, i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X, i, Board)); break; }
                else { break; }
            }

            // down
            for (int i = Y + 1; i < 8; i++) // for each piece to the right
            {
                if (Board[X, i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X, i, Board)); }
                else if (Board[X, i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X, i, Board)); break; }
                else { break; }
            }

            // up left
            for (int i = 1; i <= Math.Min(X, Y); i++)
            {
                if (Board[X - i, Y - i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X - i, Y - i, Board)); }
                else if (Board[X - i, Y - i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X - i, Y - i, Board)); break; }
                else { break; }
            }

            // up right
            for (int i = 1; i <= Math.Min(7 - X, Y); i++)
            {
                if (Board[X + i, Y - i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X + i, Y - i, Board)); }
                else if (Board[X + i, Y - i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X + i, Y - i, Board)); break; }
                else { break; }
            }

            // down left
            for (int i = 1; i <= Math.Min(X, 7 - Y); i++)
            {
                if (Board[X - i, Y + i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X - i, Y + i, Board)); }
                else if (Board[X - i, Y + i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X - i, Y + i, Board)); break; }
                else { break; }
            }

            // down right
            for (int i = 1; i <= Math.Min(7 - X, 7 - Y); i++)
            {
                if (Board[X + i, Y + i].Piece == null)  // if empty, add move
                { LegalMoves.Add(new Move(X, Y, X + i, Y + i, Board)); }
                else if (Board[X + i, Y + i].Piece.Colour != base.Colour)   // if an opp, add move and break
                { LegalMoves.Add(new Move(X, Y, X + i, Y + i, Board)); break; }
                else { break; }
            }

            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }
    }
}
