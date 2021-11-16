using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class Knight : Piece
    {
        private int[,] Offsets = new int[2, 8]
        {
            { 1, 2, 2, 1, -1, -2, -2, -1 },
            { 2, 1, -1, -2, -2, -1, 1, 2 },
        };

        public Knight(int colour) : base(colour)
        {
            base.Type = 3;
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
                    LegalMoves.Add(new Move(X, Y, X + Xo, Y + Yo, Board));
                } else { }
            }

            using (Simulator simulator = new Simulator())
            {
                LegalMoves = simulator.FilterMoves(LegalMoves, Board, base.Colour);
            }
        }
    }
}
