using ChessV2.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class Simulator : IDisposable
    {
        Piece MovingPiece;
        Piece TakenPiece;

        public void Dispose()
        {

        }

        public List<Move> FilterMoves(List<Move> moves, Square[,] Board, int Colour)
        {
            List<Move> NewList = new List<Move>();

            foreach (Move move in moves) // for every move in the list
            {
                Square[,] simBoard = Board; // get current board state

                simBoard = SimulateMove(simBoard, move); // simulate move

                // if move puts you in check remove it

                if (!MoveResultsInCheck(simBoard, move.SquareTo.Piece.Colour, move))
                {
                    NewList.Add(move);
                }

                // undo move
                simBoard = UndoMove(simBoard, move);
            }

            return NewList;
        }

        public Square[,] SimulateMove(Square[,] Board, Move move) // method to return board state after a move
        {
            MovingPiece = move.SquareFrom.Piece;
            Board[move.XFrom, move.YFrom].Piece = null;


            if (move.EnPassantType == 0) // if move not en passant
            {
                TakenPiece = Board[move.XTo, move.YTo].Piece;
                Board[move.XTo, move.YTo].AddPiece((Piece)MovingPiece);
            }
            else
            {
                TakenPiece = Board[move.XFrom + ((move.EnPassantType == 1) ? -1 : 1), move.YFrom].Piece;
                Board[move.XTo, move.YTo].AddPiece(MovingPiece);
                Board[move.XFrom + ((move.EnPassantType == 1) ? -1 : 1), move.YFrom].RemovePiece();
            }

            return Board;
        }

        public bool MoveResultsInCheck(Square[,] Board, int KingColour, Move move) // only needs board and colour
        {
            // find le king
            foreach (Square square in Board)    // foreach position in the new board
            {
                if (square.Piece is King)   // if square contains a king
                {
                    if (square.Piece.Colour == KingColour)  // of correct colour
                    {
                        return (square.Piece as King).IsInCheck(Board, square, move.SquareFrom); // return if move puts u in check
                    }
                }
            }

            return false;
        }

        public Square[,] UndoMove(Square[,] Board, Move move)
        {
            if (move.EnPassantType == 0) // if not enpassant move
            {
                // move moving piece back to old space
                Board[move.XFrom, move.YFrom].AddPiece(MovingPiece);

                // put taken piece back
                Board[move.XTo, move.YTo].Piece = TakenPiece;
            }
            else
            {
                // move moving piece back to old space
                Board[move.XFrom, move.YFrom].AddPiece(MovingPiece);
                Board[move.XFrom + +((move.EnPassantType == 1) ? -1 : 1), move.YFrom].AddPiece
                    (TakenPiece);
                Board[move.XTo, move.YTo].RemovePiece();
            }


            return Board;
        }
    }
}
