﻿using ChessV2.Pieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ChessV2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Variables
        // Visuals
        private Color _boardColor1 = new Color(240, 217, 181);
        private Color _boardColor2 = new Color(181, 136, 99);
        private Texture2D _boardTexture;
        private Texture2D _pieceTexture;
        private Texture2D _selectedTexture;
        private Texture2D _dotTexture;
        private Texture2D _circleTexture;
        private Texture2D _squaresTexture;
        private Texture2D _explosionTexture;
        private SpriteFont _font;
        private SoundEffect _boom;

        private Board board;

        public int _turn; // who go it be

        private List<Explosion> Explosions;


        // display variable
        private string _prevMove = "";
        private List<string> _moveHistory;

        // mouse processing
        private MouseState _prevMouseState;
        private MouseState _currentMouseState;


        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            board = new Board();
            Explosions = new List<Explosion>();
            _moveHistory = new List<string>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pieceTexture = Content.Load<Texture2D>("Pieces");
            _dotTexture = Content.Load<Texture2D>("Dot");
            _circleTexture = Content.Load<Texture2D>("Circle");
            _squaresTexture = Content.Load<Texture2D>("Squares");
            _explosionTexture = Content.Load<Texture2D>("Explosion");
            _font = Content.Load<SpriteFont>("Font");
            _boom = Content.Load<SoundEffect>("Boom");

            Color[] color = new Color[] { _boardColor1 };
            _boardTexture = new Texture2D(GraphicsDevice, 1, 1);
            _boardTexture.SetData<Color>(color);

            color = new Color[] { new Color(205, 210, 106) };
            _selectedTexture = new Texture2D(GraphicsDevice, 1, 1);
            _selectedTexture.SetData<Color>(color);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!board.Mate) // noone die yet
            {
                ProcessMouse(); // does what it says on the tin
                foreach (var Explosion in Explosions)   // update explosion/s
                {
                    Explosion.Update();
                    if (Explosion.IsDisposed)
                    { Explosions.Remove(Explosion); break; }
                }
            }

            base.Update(gameTime);
        }

        private void ProcessMouse()
        {
            _currentMouseState = Mouse.GetState();  // get current mouse state

            if (_currentMouseState.LeftButton == ButtonState.Pressed
                && _prevMouseState.LeftButton == ButtonState.Released)  // he will never be clickin
            {
                int X = (int)_currentMouseState.X / 100;
                int Y = (int)_currentMouseState.Y / 100;    // get x/y positions of mouse on board
                if (board.selectedSquare != null)   // if you have selected a piece before
                {
                    if (X >= 0 && X < 8 && Y >= 0 && Y < 8)
                    {
                        if (board.Squares[X, Y].ContainsPiece())
                        {
                            if (board.Squares[X, Y].Piece.Colour == _turn)
                            { TrySelect(X, Y); }
                            else
                            { TryMove(X, Y); }
                        }
                        else { TryMove(X, Y); }
                    }
                }
                else
                {
                    TrySelect(X, Y);
                }
            }

            _prevMouseState = _currentMouseState;
        }

        private void TrySelect(int X, int Y)
        {
            try
            {
                board.SelectSquare(X, Y, _turn);
            }
            catch { }
        }

        private void TryMove(int X, int Y)
        {
            // if a piece is selected and move is on board
            if (board.selectedSquare.ContainsPiece() && X >= 0 && X < 8 && Y >= 0 && Y < 8)
            {
                foreach (Move move in board.selectedSquare.Piece.LegalMoves)    // foreach legal move of piece
                {
                    if (move.XTo == X & move.YTo == Y)  // if move is in legal moves
                    {
                        if (move.EnPassantType != 0) // if move is enpassant
                        {
                            board.MovePieceEnPassant(move, move.EnPassantType);
                            _turn = 1 - _turn;
                            _prevMove = move.MoveName;
                            _moveHistory.Add(_prevMove);
                            Explosions.Add(new Explosion(_explosionTexture, 
                                move.XFrom + ((move.EnPassantType == 1) ? -1 : 1), move.YFrom));

                            _boom.Play();

                            break;
                        }
                        else // if move not enpassant
                        {
                            board.MovePiece(move); // move
                            _turn = 1 - _turn;
                            _prevMove = move.MoveName;
                            _moveHistory.Add(_prevMove);
                            break;
                        }
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_boardColor2);

            _spriteBatch.Begin();
            // ====================================

            // does what it says on the tin tbh
            DrawSquares();
            DrawSelectedPiece();
            DrawCheckSquare();
            DrawPrevMove();
            DrawLegalMoves();
            DrawPieces();
            DrawVariables();
            DrawExplosions();

            // ====================================
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void DrawSquares()  // method to draw chess board
        {
            for (int x = 0; x < 8; x++) // for each column
            {
                for (int y = 0; y < 8; y++) // for each row
                {
                    if ((x + y) % 2 == 0)   // if x + y is a multiple of 2 (every other square)
                    {
                        _spriteBatch.Draw(_boardTexture, new Vector2(x * 100, y * 100),
                            new Rectangle(0, 0, 100, 100), Color.White);    // draw square
                    }   
                }
            }
        }

        private void DrawSelectedPiece()    // method to highlight selected piece
        {
            if (board.selectedSquare != null)   // if a square has been selected
            {
                var x = board.selectedSquare.File;
                var y = board.selectedSquare.Rank;
                _spriteBatch.Draw(_selectedTexture, new Rectangle(x * 100, y * 100, 100, 100),
                Color.DarkOliveGreen);  // highlight selected piece
            }
        }
        public void DrawCheckSquare()   // method to highlight a king in check
        {
            (int X, int Y) = board.checkSquare; // get position of king in check
            _spriteBatch.Draw(_selectedTexture, new Rectangle(X * 100, Y * 100, 100, 100), Color.Red); 
            // draw red square
        }

        public void DrawPrevMove()
        {
            (int XF, int YF) = board.prevSquareFrom;
            (int XT, int YT) = board.prevSquareTo;
            _spriteBatch.Draw(_selectedTexture, new Rectangle(XF * 100, YF * 100, 100, 100), Color.White);
            _spriteBatch.Draw(_selectedTexture, new Rectangle(XT * 100, YT * 100, 100, 100), Color.White);
        }

        private void DrawLegalMoves()  // method to draw legal moves for selected piece
        {
            if (board.selectedSquare != null) // if a piece has even been selected
            {
                if (board.selectedSquare.ContainsPiece()) // if the square selected has a piece in it
                {
                    foreach(Move move in board.selectedSquare.Piece.LegalMoves) // for each of said pieces legal moves
                    {
                        if (move.SquareTo.ContainsPiece())  // if move is a taking move, draw circle, else, draw dot
                        {
                            _spriteBatch.Draw(_circleTexture, new Rectangle(move.XTo * 100, move.YTo * 100, 100, 100),
                                new Rectangle(0, 0, 200, 200), Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(_dotTexture, new Rectangle(move.XTo * 100, move.YTo * 100, 100, 100),
                            new Rectangle(0, 0, 200, 200), Color.White);
                        }
                    }
                }
            }
        }

        private void DrawPieces()   // method to draw eac hpiece present on the board
        {   
            foreach(Square square in board.Squares) // for every position on the board
            {
                if (square.ContainsPiece())   // if it contains a piece
                {
                    (var x, var y) = square.GetPosition();  // get its x y position
                    var _type = square.Piece.Type;  // get piece type
                    var _colour = square.Piece.Colour;  // get piece colour
                    _spriteBatch.Draw(_pieceTexture, new Rectangle(x * 100, y * 100, 100, 100),
                    new Rectangle(_type * 200, _colour * 200, 200, 200), Color.White);
                    // draw piece
                }
            }
        }

        private void DrawVariables()
        {
            // OLD
            //{
            //    if (board.selectedSquare != null)
            //    {
            //        if (board.selectedSquare.ContainsPiece())
            //        {

            //            _spriteBatch.DrawString(_font, "File: " +
            //                board.selectedSquare.Piece.File.ToString(), new Vector2(0, 820), Color.White);

            //            _spriteBatch.DrawString(_font, "Rank: " +
            //                board.selectedSquare.Piece.Rank.ToString(), new Vector2(0, 840), Color.White);

            //            _spriteBatch.DrawString(_font, "CBEP: " +
            //                board.selectedSquare.Piece.CanBeEnPassant.ToString(), new Vector2(0, 860), Color.White);

            //            _spriteBatch.DrawString(_font, "Can Castle: " +
            //                board.selectedSquare.Piece.CanCastle.ToString(), new Vector2(300, 880), Color.White);
            //        }
            //    }
            //    _spriteBatch.DrawString(_font, "Prev Move: " +
            //        _prevMove, new Vector2(0, 880), Color.White);

            //    _spriteBatch.DrawString(_font, "Check: " +
            //        board.Check.ToString(), new Vector2(300, 800), Color.White);

            //    _spriteBatch.DrawString(_font, "Mate: " +
            //        board.Mate.ToString(), new Vector2(300, 820), Color.White);
            //} 

            for (int i = 0; i < _moveHistory.Count; i++)
            {
                _spriteBatch.DrawString(_font, _moveHistory[i], new Vector2(
                    810 + ((i % 2) * 90), 10 + ((i / 2) * 30)), (i % 2 == 0) ? Color.White : Color.Black);
            }
        }
        private void DrawExplosions()   // does exactly what it says on the tin
        {
            foreach (var Explosion in Explosions)
            {
                Explosion.Draw(_spriteBatch);
            }
        }
    }
}
