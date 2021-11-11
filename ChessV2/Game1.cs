using ChessV2.Pieces;
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

        // mouse processing
        private MouseState _prevMouseState;
        private MouseState _currentMouseState;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 1000;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            board = new Board();
            Explosions = new List<Explosion>();
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
            if (board.selectedSquare.Piece != null && X >= 0 && X < 8 && Y >= 0 && Y < 8)
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

            DrawSquares();
            DrawSelectedPiece();
            DrawCheckSquare();
            DrawPrevMove();
            DrawCurrentPieceAttacks();
            DrawPieces();
            DrawVariables();
            DrawExplosions();

            // ====================================
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void DrawSquares()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        _spriteBatch.Draw(_boardTexture, new Vector2(x * 100, y * 100),
                            new Rectangle(0, 0, 100, 100), Color.White);
                    }
                }
            }
            _spriteBatch.Draw(_squaresTexture, Vector2.Zero, Color.White * 0.8f);
        }

        private void DrawSelectedPiece()
        {
            if (board.selectedSquare != null)
            {
                var x = board.selectedSquare.File;
                var y = board.selectedSquare.Rank;
                _spriteBatch.Draw(_selectedTexture, new Rectangle(x * 100, y * 100, 100, 100),
                Color.DarkOliveGreen);
            }
        }
        public void DrawCheckSquare()
        {
            (int X, int Y) = board.checkSquare;
            _spriteBatch.Draw(_selectedTexture, new Rectangle(X * 100, Y * 100, 100, 100), Color.Red);
        }

        public void DrawPrevMove()
        {
            (int XF, int YF) = board.prevSquareFrom;
            (int XT, int YT) = board.prevSquareTo;
            _spriteBatch.Draw(_selectedTexture, new Rectangle(XF * 100, YF * 100, 100, 100), Color.White);
            _spriteBatch.Draw(_selectedTexture, new Rectangle(XT * 100, YT * 100, 100, 100), Color.White);
        }

        private void DrawCurrentPieceAttacks()
        {
            if (board.selectedSquare != null)
            {
                if (board.selectedSquare.Piece != null)
                {
                    foreach(Move move in board.selectedSquare.Piece.LegalMoves)
                    {
                        if (move.SquareTo.ContainsPiece())
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

        private void DrawPieces()
        {
            foreach(Square square in board.Squares)
            {
                if (square.Piece != null)
                {
                    (var x, var y) = square.GetPosition();
                    var _type = square.Piece.Type;
                    var _colour = square.Piece.Colour;
                    _spriteBatch.Draw(_pieceTexture, new Rectangle(x * 100, y * 100, 100, 100),
                    new Rectangle(_type * 200, _colour * 200, 200, 200), Color.White);
                }
            }
        }

        private void DrawVariables()
        {
            if (board.selectedSquare != null)
            {
                if (board.selectedSquare.Piece != null)
                {
                    //_spriteBatch.DrawString(_font,
                    //    board.selectedSquare.Piece.LegalMoves.ToString(),
                    //    new Vector2(0, 800), Color.White);

                    _spriteBatch.DrawString(_font, "File: " +
                        board.selectedSquare.Piece.File.ToString(), new Vector2(0, 820), Color.White);

                    _spriteBatch.DrawString(_font, "Rank: " +
                        board.selectedSquare.Piece.Rank.ToString(), new Vector2(0, 840), Color.White);

                    _spriteBatch.DrawString(_font, "CBEP: " +
                        board.selectedSquare.Piece.CanBeEnPassant.ToString(), new Vector2(0, 860), Color.White);
                }
            }
            _spriteBatch.DrawString(_font, "Prev Move: " +
                _prevMove, new Vector2(0, 880), Color.White);

            _spriteBatch.DrawString(_font, "Check: " +
                board.Check.ToString(), new Vector2(300, 800), Color.White);

            _spriteBatch.DrawString(_font, "Mate: " +
                board.Mate.ToString(), new Vector2(300, 820), Color.White);


        }
        private void DrawExplosions()
        {
            foreach (var Explosion in Explosions)
            {
                Explosion.Draw(_spriteBatch);
            }
        }
    }
}
