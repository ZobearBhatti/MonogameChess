using ChessV2.Pieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessV2
{
    internal class AnimatingPiece
    {
        private Texture2D texture;

        private float X;
        private float Y;

        private float XFrom;
        private float YFrom;
        private float XTo;
        private float YTo;

        private float timer;

        private Move Move;

        private int Colour;
        private int Type;

        public bool Animating { get; private set; }

        public AnimatingPiece(Texture2D _texture, Move move, Piece piece)
        {
            texture = _texture; Move = move;
            XFrom = move.XFrom * 100; XTo = move.XTo * 100;
            YFrom = move.YFrom * 100; YTo = move.YTo * 100;
            X = XFrom; Y = YFrom;
            timer = 0f;
            Colour = piece.Colour;
            Type = piece.Type;

            Animating = true;
        }
        
        public Move GetMove()
        {
            return Move;
        }


        public void Update()
        {
            timer += 0.08f; // timer will represent a percentage

            float t = (timer == 1) ? 1f : (float)(1 - Math.Pow(2, -10 * timer));

            X = XFrom + (t * (XTo - XFrom));
            Y = YFrom + (t * (YTo - YFrom));

            if (timer >= 0.99f)
            {
                Animating = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)X, (int)Y, 100, 100),
                new Rectangle(Type * 200, Colour * 200, 200, 200), Color.White);
        }
    }

    public class Board
    {

        #region Variables

        private AnimatingPiece animPiece;
        private Texture2D _texture;

        public Square[,] Squares { get; set; }
        public Square selectedSquare { get; private set; }
        public (int, int) checkSquare { get; private set; } // field is only used for visuals so tuple fine
        public (int, int) prevSquareFrom { get; private set; } // field is only used for visuals so tuple fine
        public (int, int) prevSquareTo { get; private set; } // field is only used for visuals so tuple fine

        public bool Check { get; set; }
        public bool Mate { get; set; }
        public bool Draw { get; set; }

        private int _fiftyMoveCounter;

        private string _drawType;

        public bool RequirePromotion { get; set; }

        private List<Move> MoveList { get; }

        private List<Square[,]> BoardStates;

        public Move prevMove { get; private set; }

        #endregion

        public Board(Texture2D texture)
        {
            _texture = texture;
            
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
            //Squares[0, 2].AddPiece(new Rook(0, "a")); Squares[6, 7].AddPiece(new Rook(0, "h"));
            //Squares[0, 1].AddPiece(new Rook(0));

            // knights
            //Squares[1, 0].AddPiece(new Knight(1)); Squares[6, 0].AddPiece(new Knight(1));
            //Squares[1, 7].AddPiece(new Knight(0)); // Squares[6, 7].AddPiece(new Knight(0));

            // bishops
            //Squares[2, 0].AddPiece(new Bishop(1)); Squares[5, 0].AddPiece(new Bishop(1));
            //Squares[2, 7].AddPiece(new Bishop(0)); Squares[5, 7].AddPiece(new Bishop(0));

            // kings, queens
            //Squares[3, 0].AddPiece(new Queen(1));   // bq
            Squares[7, 1].AddPiece(new King(1));    // bk
            //Squares[6, 6].AddPiece(new Queen(0));   // wq
            Squares[4, 7].AddPiece(new King(0));    // wk

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

        Piece _piece; // piece

        public void MovePiece(Move move)
        {
            Check = false;
            bool EP = (move.EnPassantType != 0) ? true : false;
            move.SquareTo.RemovePiece();
            Square From = move.SquareFrom; Square To = move.SquareTo;
            var piece = From.Piece;
            _piece = piece;

            prevMove = move;
            prevSquareFrom = (move.SquareFrom.File, move.SquareFrom.Rank);
            prevSquareTo = (move.SquareTo.File, move.SquareTo.Rank);

            if (!EP) // non enpassant moves
            {
                if (!move.SquareTo.ContainsPiece() && !(move.SquareFrom.Piece is Pawn)) // if move is not a taking or pawn move
                {
                    _fiftyMoveCounter++; // increment stalemate counter
                }

                From.RemovePiece();

                ClearEnPassant();

                // if move is a pawn jumping 2 ranks
                if (piece is Pawn &&
                    ((From.Rank == 1 && To.Rank == 3) || (From.Rank == 6 && To.Rank == 4)))
                {
                    _piece.CanBeEnPassant = true;
                }

                // if piece is promoting
                else if (_piece is Pawn && (To.Rank == 0 || To.Rank == 7))
                {
                    RequirePromotion = true;
                }

                else if (piece is King && move.IsCastle) // CASTLING
                {
                    if (To.File > 4) // right side
                    {
                        Squares[5, To.Rank].AddPiece(new Rook(_piece.Colour));
                        Squares[7, To.Rank].RemovePiece();
                    }
                    else // left side
                    {
                        Squares[3, To.Rank].AddPiece(new Rook(piece.Colour));
                        Squares[0, To.Rank].RemovePiece();
                    }
                }

                else if (piece is King || piece is Rook)  // if a king or pawn is moving
                {
                    _piece.CanCastle = false;
                }
            }

            else
            {
                From.RemovePiece(); // To.AddPiece(piece);
                Squares[From.File + ((move.EnPassantType == 1) ? -1 : 1), From.Rank].RemovePiece();

                ClearEnPassant();
            }

            //move.SquareTo.AddPiece(_piece);
            //CheckForCheck(move, move.SquareTo);
            //move.SquareTo.RemovePiece();

            animPiece = new AnimatingPiece(_texture, move, piece);
        }

        private void AfterMove(Move move)
        {
            if (!RequirePromotion)
            {
                MoveList.Add(move); // only add the move straight away if its not promotion
                move.SquareTo.AddPiece(_piece);
                CheckForCheck(move, move.SquareTo);
            }

            if (!Mate && _fiftyMoveCounter == 100) // draw by fifty move rule
            {
                Draw = true; _drawType = "50 move rule";
            }

            BoardStates.Add(Squares);
        }

        private void CheckForCheck(Move move, Square To)
        {
            // if Move results in check
            foreach (Square square in Squares)    // foreach position in the new board
            {
                if (square.Piece is King)   // if square contains a king
                {
                    if (square.Piece.Colour == 1 - _piece.Colour)  // of correct colour
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
                    if (sq.Piece.Colour != _piece.Colour) // if its an opp
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
                    if (sq.Piece.Colour != _piece.Colour)
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

            prevMove.SetPromoteName(choice);
            MoveList.Add(prevMove);

            int x = prevSquareTo.Item1; int y = prevSquareTo.Item2;

            Squares[x, y].RemovePiece();

            switch (choice)
            {
                case 0:
                    _piece = (new Queen(colour)); break;
                case 1:
                    _piece = (new Bishop(colour)); break;
                case 2:
                    _piece = (new Knight(colour)); break;
                case 3:
                    _piece = (new Rook(colour)); break;
            }

            //CheckForCheck(MoveList.Last(), MoveList.Last().SquareTo);
        }

        private void ClearEnPassant()
        {
            foreach (Square square in Squares)
            {
                if (square.Piece is Pawn)
                { square.Piece.CanBeEnPassant = false; }
            }
        }

        public string GetDrawType()
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
                if (!(WhitePieces.Contains(1) || WhitePieces.Contains(4))
                    && !(BlackPieces.Contains(1) || BlackPieces.Contains(4))) // if neither side has a rook or queen
                {
                    Draw = true; _drawType = "Insufficient material"; return;
                }
            }
        }

        public void Update()
        {
            if (!(animPiece is null)) // if we actaully HAVE a piece animating
            {
                if (!animPiece.Animating) // if finished animating
                {
                    AfterMove(animPiece.GetMove());
                    animPiece = null;
                }
                else
                {
                    animPiece.Update();
                }
            }
        }

        public void Animate(SpriteBatch spriteBatch)
        {
            animPiece.Draw(spriteBatch);
        }

        public bool IsAnimating()
        {
            return (animPiece != null);
        }
    }
}
