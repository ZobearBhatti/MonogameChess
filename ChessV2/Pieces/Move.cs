using ChessV2.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class Move
    {
        public Square SquareFrom { get; set; }
        public Square SquareTo { get; set; }
        public int XFrom { get; set; }
        public int YFrom { get; set; }
        public int XTo { get; set; }
        public int YTo { get; set; }
        public int EnPassantType { get; set; }
        public bool IsCastle { get; set; }
        public string MoveName { get; set; }

        private Square[,] board;

        public Move(int FileFrom, int RankFrom, int FileTo, int RankTo, Square[,] Board, int enPassantType, bool isCastle)
        {
            SquareFrom = Board[FileFrom, RankFrom];
            SquareTo = Board[FileTo, RankTo];
            XFrom = FileFrom; YFrom = RankFrom;
            XTo = FileTo; YTo = RankTo;
            EnPassantType = enPassantType;
            board = Board;
            IsCastle = isCastle;
            if (IsCastle)
            {
                MoveName = (FileTo < 4) ? "O-O-O" : "O-O";
            }
            else
            {
                MoveName = GenerateMoveName();
            }
        }

        private string GenerateMoveName()
        {
            string Return = "";

            var piece = board[XFrom, YFrom].Piece;

            switch(piece.Type) // generate piece name:
            {
                case 0:
                    Return += "K"; break;
                case 1:
                    Return += "Q"; break;
                case 2:
                    Return += "B"; break;
                case 3:
                    Return += "N"; break;
                case 4:
                    Return += "R" + (piece as Rook).RookFile; break;
                case 5: // pawn file is only added if pawn is taking or enpassanting
                    if (SquareTo.ContainsPiece() || EnPassantType != 0)
                    {
                        switch (XFrom)
                        {
                            case 0: Return += "a"; break;
                            case 1: Return += "b"; break;
                            case 2: Return += "c"; break;
                            case 3: Return += "d"; break;
                            case 4: Return += "e"; break;
                            case 5: Return += "f"; break;
                            case 6: Return += "g"; break;
                            case 7: Return += "h"; break;
                        }
                    }
                    break;
            }

            if (SquareTo.ContainsPiece() || EnPassantType != 0)
            {
                Return += "x";
            }

            // add "to" pos
            switch (XTo)
            {
                case 0: Return += "a"; break;
                case 1: Return += "b"; break;
                case 2: Return += "c"; break;
                case 3: Return += "d"; break;
                case 4: Return += "e"; break;
                case 5: Return += "f"; break;
                case 6: Return += "g"; break;
                case 7: Return += "h"; break;
            }
            Return += (8 - YTo).ToString();

            return Return;
        }
    }
}
