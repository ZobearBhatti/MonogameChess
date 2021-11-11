using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class Rook : Piece
    {
        public bool CanCastle { get; set; }
        public Rook(int colour) : base(colour)
        {
            base.Type = 4; CanCastle = true;
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

            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }

        public override void GenerateAttacks()
        {

        }
    }
}
