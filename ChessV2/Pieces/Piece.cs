using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2.Pieces
{
    public class Piece
    {
        public int Colour { get; set; }
        public int Type { get; set; }

        public List<Move> LegalMoves { get; set; }

        public int File { get; set; }
        public int Rank { get; set; }

        // Piece specific stuff
        public bool CanBeEnPassant { get; set; }

        public bool CanCastle { get; set; }

        public Piece(int colour)    // used normally
        {
            Colour = colour; LegalMoves = new List<Move>();
        }

        public Piece(int colour, string File) // used for rooks
        {
            Colour = colour; LegalMoves = new List<Move>();
        }

        public virtual void GenerateLegalMoves(Square[,] Board)
        {
            
        }
    }
}
