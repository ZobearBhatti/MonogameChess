using ChessV2.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Check { get; set; }
        public bool Mate { get; set; }
        public bool Draw { get; set; }

        private int _fiftyMoveCounter;

        private int _repetitionCounter;

        private string _drawType;

        public bool RequirePromotion { get; set; }

        private List<Move> MoveList { get; }

        private List<Square[,]> BoardStates;

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
            Check = false;
            Mate = false;
            checkSquare = (-1, -1);
            prevSquareFrom = (-1, -1);
            prevSquareTo = (-1, -1);
            MoveList = new List<Move>();
            BoardStates = new List<Square[,]>();
            RequirePromotion = false;
            _fiftyMoveCounter = 0; // 50 move rule counter

            DefaultPositions();

            //CustomPositions();
        }

        private void DefaultPositions()
        {
            for (int x = 0; x < 8; x++) // pawns
            {
                Squares[x, 1].AddPiece(new Pawn(1));
                Squares[x, 6].AddPiece(new Pawn(0));
            }

            // rooks
            Squares[0, 0].AddPiece(new Rook(1, "a")); Squares[7, 0].AddPiece(new Rook(1, "h"));
            Squares[0, 7].AddPiece(new Rook(0, "a")); Squares[7, 7].AddPiece(new Rook(0, "h"));

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

        private void CustomPositions()
        {
            Squares[1, 1].AddPiece(new Pawn(0));
            //Squares[7, 7].AddPiece(new Knight(1));

            // rooks
            //Squares[0, 0].AddPiece(new Rook(1, "a")); Squares[7, 0].AddPiece(new Rook(1, "h"));
            //Squares[0, 7].AddPiece(new Rook(0, "a")); Squares[3, 1].AddPiece(new Rook(0, "h"));

            // knights
            //Squares[1, 0].AddPiece(new Knight(1)); Squares[6, 0].AddPiece(new Knight(1));
            Squares[1, 7].AddPiece(new Knight(0)); // Squares[6, 7].AddPiece(new Knight(0));

            // bishops
            //Squares[2, 0].AddPiece(new Bishop(1)); Squares[5, 0].AddPiece(new Bishop(1));
            //Squares[2, 7].AddPiece(new Bishop(0)); Squares[5, 7].AddPiece(new Bishop(0));

            // kings, queens
            //Squares[3, 0].AddPiece(new Queen(1));   // bq
            Squares[7, 0].AddPiece(new King(1));    // bk
            //Squares[6, 6].AddPiece(new Queen(0));   // wq
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
            MoveList.Add(move);
            Square From = move.SquareFrom; Square To = move.SquareTo;
            var piece = From.Piece;

            if (!move.SquareTo.ContainsPiece() && !(move.SquareFrom.Piece is Pawn)) // if move is not a taking or pawn move
            {
                _fiftyMoveCounter++; // increment sstalemate counter
            }

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
                RequirePromotion = true;
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

            MoveList.Add(move);


            CheckForCheck(move, To);

            if (!Mate && _fiftyMoveCounter == 100) // draw by fifty move rule
            {
                Draw = true; _drawType = "50 move rule";
            }

            BoardStates.Add(Squares);
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

            CheckForCheck(move, To);
            MoveList.Add(move);
        }

        private void CheckForCheck(Move move, Square To)
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
                            CheckForMate(move, To, square);
                            if (Mate) { return; }
                        }
                        else // king is NOT in check => check for stalemate
                        {
                            CheckForStaleMate(move, To);
                            if (Draw) { _drawType = "Stalemate"; return; }
                        }
                    }
                }
            }
            CheckForInsufficientMaterial();
        }

        private void CheckForStaleMate(Move move, Square To)
        {
            Check = false; Mate = false; checkSquare = (-1, -1);
            string _oldMoveName = move.MoveName; // get old move name
            foreach (Square sq in Squares)  // for each square on the board
            {
                if (sq.ContainsPiece()) // if it has a piece
                {
                    if (sq.Piece.Colour != To.Piece.Colour) // if its an opp
                    {
                        sq.Piece.GenerateLegalMoves(Squares); // get its legal moves
                        if (sq.Piece.LegalMoves.Count > 0)
                        {
                            Draw = false;
                            move.MoveName = _oldMoveName; break;
                        }
                        Draw = true; move.MoveName = "1/2 - 1/2";
                    }
                }
            }
        }

        private void CheckForMate(Move move, Square To, Square square)
        {
            checkSquare = (square.File, square.Rank);   // get square of king in check
            Check = true;   // check lmao
            move.MoveName += "+";   // add this to the move name
            foreach (Square sq in Squares)  // for each square on the board
            {
                if (sq.ContainsPiece()) // if it has a piece
                {
                    if (sq.Piece.Colour != To.Piece.Colour)
                    {
                        sq.Piece.GenerateLegalMoves(Squares);
                        if (sq.Piece.LegalMoves.Count > 0)
                        {
                            move.MoveName = move.MoveName.Substring(0, move.MoveName.Length - 1);
                            move.MoveName += "+";
                            Mate = false; break;
                        }
                        move.MoveName = move.MoveName.Substring(0, move.MoveName.Length - 1);
                        move.MoveName += "#";
                        Mate = true;
                    }
                }
            }
        }

        public void Promote(int choice, int colour)
        {
            int x = prevSquareTo.Item1; int y = prevSquareTo.Item2;

            Squares[x, y].RemovePiece();

            switch (choice)
            {
                case 0:
                    Squares[x, y].AddPiece(new Queen(colour)); break;
                case 1:
                    Squares[x, y].AddPiece(new Bishop(colour)); break;
                case 2:
                    Squares[x, y].AddPiece(new Knight(colour)); break;
                case 3:
                    Squares[x, y].AddPiece(new Rook(colour)); break;
            }

            CheckForCheck(MoveList.Last(), MoveList.Last().SquareTo);
        }

        private void ClearEnPassant()
        {
            foreach (Square square in Squares)
            {
                if (square.Piece is Pawn)
                { square.Piece.CanBeEnPassant = false; }
            }
        }

        public string GetStaleMateType()
        {
            return _drawType;
        }

        private void CheckForInsufficientMaterial()
        {
            /* The possibilties for this are:
             * BOTH sides have any of:
             * - A lone king
             * - A king and a bishop
             * - A king and a knight
             * OR one side has a king and two knights and the other has just a king
             * if ANY pawns are in play, there is sufficient material*/
            List<int> WhitePieces = new List<int>() { 0 };
            List<int> BlackPieces = new List<int>() { 0 };

            foreach(Square sq in Squares)
            {
                if (sq.ContainsPiece())
                {
                    if (!(sq.Piece is King))
                    {
                        if (sq.Piece.Colour == 0) // white
                        {
                            if (sq.Piece.Type == 5) { return; }
                            WhitePieces.Add(sq.Piece.Type);
                        }
                        else
                        {
                            if (sq.Piece.Type == 5) { return; } // no pawns allowed
                            BlackPieces.Add(sq.Piece.Type);
                        }
                    }
                }
            }   // KEEP TRACK OF EVERY PIECE ON THE BOARD

            WhitePieces.Sort(); BlackPieces.Sort();
            // every possible combination (theres not that many)

            // k && 2 n against k
            if ((Enumerable.SequenceEqual(WhitePieces, new List<int>() { 0, 3, 3 }) && BlackPieces.Count == 1) ||
                (Enumerable.SequenceEqual(BlackPieces, new List<int>() { 0, 3, 3 }) && WhitePieces.Count == 1))
            { Draw = true; _drawType = "Insufficient material"; return; }

            if (WhitePieces.Count < 3 && BlackPieces.Count < 3) // having more than 2 pieces otherwise dont count
            {
                if (!(WhitePieces.Contains(1) && WhitePieces.Contains(4))
                    && !(BlackPieces.Contains(1) && BlackPieces.Contains(4))) // if neither side has a rook or queen
                {
                    Draw = true; _drawType = "Insufficient material"; return;
                }
            }
        }
    }
}
