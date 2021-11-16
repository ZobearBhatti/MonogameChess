using ChessV2.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class Board
    {
        public Square[,] Squares { get; set; }
        public Square selectedSquare { get; set; }

        public (int, int) checkSquare { get; set; } // field is only used for visuals so tuple fine
        public (int, int) prevSquareFrom { get; set; } // field is only used for visuals so tuple fine
        public (int, int) prevSquareTo { get; set; } // field is only used for visuals so tuple fine

        public Move PrevMove { get; set; }

        private bool _check;
        private bool _mate;

        public bool Check { get { return _check; } set { } }
        public bool Mate { get { return _mate; } set { } }

        public Board()
        {
            Squares = new Square[8, 8];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Squares[x, y] = new Square(x, y);
                }
            }
            selectedSquare = null;
            DefaultPositions();
            _check = false;
            _mate = false;
            checkSquare = (-1, -1);
            prevSquareFrom = (-1, -1);
            prevSquareTo = (-1, -1);
        }

        public void DefaultPositions()
        {
            for (int x = 0; x < 8; x++) // pawns
            {
                Squares[x, 1].AddPiece(new Pawn(1));
                Squares[x, 6].AddPiece(new Pawn(0));
            }
            // rooks
            Squares[0, 0].AddPiece(new Rook(1)); Squares[7, 0].AddPiece(new Rook(1));
            Squares[0, 7].AddPiece(new Rook(0)); Squares[7, 7].AddPiece(new Rook(0));

            // knights
            Squares[1, 0].AddPiece(new Knight(1)); Squares[6, 0].AddPiece(new Knight(1));
            Squares[1, 7].AddPiece(new Knight(0)); Squares[6, 7].AddPiece(new Knight(0));

            // bishops
            Squares[2, 0].AddPiece(new Bishop(1)); Squares[5, 0].AddPiece(new Bishop(1));
            Squares[2, 7].AddPiece(new Bishop(0)); Squares[5, 7].AddPiece(new Bishop(0));

            // kings, queens
            Squares[3, 0].AddPiece(new Queen(1));   // bq
            Squares[4, 0].AddPiece(new King(1));    // bk
            Squares[3, 7].AddPiece(new Queen(0));   // wq
            Squares[4, 7].AddPiece(new King(0));    // wk

        }

        public Square GetSquare(int file, int rank)
        {
            return Squares[file, rank];
        }

        public void SelectSquare(int x, int y, int colour)
        {
            if (Squares[x, y].ContainsPiece())
            {
                if (Squares[x, y].Piece.Colour == colour)
                {
                    selectedSquare = Squares[x, y];
                    Squares[x, y].Piece.GenerateLegalMoves(Squares);
                }
            }
        }

        public void MovePiece(Move move)
        {
            Square From = move.SquareFrom; Square To = move.SquareTo;
            var piece = From.Piece;
            From.RemovePiece(); To.AddPiece(piece);

            ClearEnPassant();

            // if move is a pawn jumping 2 ranks
            if (To.Piece is Pawn &&
                ((From.Rank == 1 && To.Rank == 3) || (From.Rank == 6 && To.Rank == 4)))
            {
                To.Piece.CanBeEnPassant = true;
            }

            // if piece is promoting
            else if (To.Piece is Pawn && (To.Rank == 0 || To.Rank == 7))
            {
                int c = To.Piece.Colour;

                Squares[To.File, To.Rank].RemovePiece();
                // add queen by default === CHANGE LATER

                Squares[To.File, To.Rank].AddPiece(new Queen(c));
            }

            else if (To.Piece is King && move.IsCastle) // CASTLING
            {
                if (To.File > 4) // right side
                {
                    Squares[5, To.Rank].AddPiece(new Rook(To.Piece.Colour));
                    Squares[7, To.Rank].RemovePiece();
                }
                else // left side
                {
                    Squares[3, To.Rank].AddPiece(new Rook(To.Piece.Colour));
                    Squares[0, To.Rank].RemovePiece();
                }
            }

            else if (To.Piece is King || To.Piece is Rook)  // if a king or pawn is moving
            {
                To.Piece.CanCastle = false;
            }


            prevSquareFrom = (move.SquareFrom.File, move.SquareFrom.Rank);
            prevSquareTo = (move.SquareTo.File, move.SquareTo.Rank);
            CheckForCheckMate(move, To);
        }

        public void MovePieceEnPassant(Move move, int Offset)
        {
            Square From = move.SquareFrom; Square To = move.SquareTo;
            var piece = From.Piece;
            From.RemovePiece(); To.AddPiece(piece);
            Squares[From.File + ((Offset == 1) ? -1 : 1), From.Rank].RemovePiece();

            ClearEnPassant();

            prevSquareFrom = (move.SquareFrom.File, move.SquareFrom.Rank);
            prevSquareTo = (move.SquareTo.File, move.SquareTo.Rank);

            CheckForCheckMate(move, To);
        }

        private void CheckForCheckMate(Move move, Square To)
        {
            // if Move results in check
            foreach (Square square in Squares)    // foreach position in the new board
            {
                if (square.Piece is King)   // if square contains a king
                {
                    if (square.Piece.Colour == 1 - To.Piece.Colour)  // of correct colour
                    {
                        if ((square.Piece as King).IsInCheck(Squares, square, move.SquareTo)) // return if move puts u in check
                        {
                            checkSquare = (square.File, square.Rank);
                            _check = true;
                            move.MoveName += "+";
                            foreach (Square sq in Squares)
                            {
                                if (sq.ContainsPiece())
                                {
                                    if (sq.Piece.Colour != To.Piece.Colour)
                                    {
                                        sq.Piece.GenerateLegalMoves(Squares);
                                        if (sq.Piece.LegalMoves.Count > 0)
                                        {
                                            _mate = false; break;
                                        }
                                        _mate = true;
                                    }
                                }
                            }
                        }
                        else { _check = false; _mate = false; checkSquare = (-1, -1); }
                    }
                }
            }
        }

        private void ClearEnPassant()
        {
            foreach (Square square in Squares)
            {
                if (square.Piece is Pawn)
                { square.Piece.CanBeEnPassant = false; }
            }
        }
    }
}
