using ChessV2.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class Square
    {
        public Piece Piece { get; set; }

        public int Rank { get; set; }
        public int File { get; set; }

        public Square(int file, int rank)
        {
            File = file; Rank = rank; Piece = null;
        }

        public void RemovePiece()
        {
            Piece = null;
        }

        public (int, int) GetPosition()
        {
            return (File, Rank);
        }

        public void AddPiece(Piece piece)
        {
            Piece = piece;
            Piece.Rank = Rank; Piece.File = File;
        }

        public bool ContainsPiece()
        {
            return (this.Piece == null) ? false : true;
        }
    }
}
