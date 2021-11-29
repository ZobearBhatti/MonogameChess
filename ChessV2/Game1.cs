using ChessV2.Pieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private Texture2D _windowTexture;
        private SpriteFont _font;
        private SpriteFont _winFont;
        private SoundEffect _boom;

        private Board board; // Board class, represents the board surprisingly

        public int _turn; // whose turn it is 0-1

        private List<Explosion> Explosions; // now that i think about it why did i make this a list

        // display variable
        private string _prevMove = "";
        private List<string> _moveHistory;

        // mouse processing
        private MouseState _prevMouseState;
        private MouseState _currentMouseState;

        private WinWindow winWindow;
        private PromotionWindow promWindow;

        #endregion

        public Game1()  // default code to initialise window
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()    // run once upon program startup
        {
            Explosions = new List<Explosion>(); // create new list
            base.Initialize();
        }

        private void SetUpGame()
        {
            board = new Board(_pieceTexture);    // create new board
            _moveHistory = new List<string>();  // reset move history
            _turn = 0;  // white's turn
            winWindow = null;
            promWindow = null;  // nul windows that dont exist just yet

            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()   // method to load content from the content pipeline
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice); // default code: initialise spritebatch

            _pieceTexture = Content.Load<Texture2D>("Pieces");
            _dotTexture = Content.Load<Texture2D>("Dot");
            _circleTexture = Content.Load<Texture2D>("Circle");
            _squaresTexture = Content.Load<Texture2D>("Squares");
            _explosionTexture = Content.Load<Texture2D>("Explosion");
            _windowTexture = Content.Load<Texture2D>("Window");
            _font = Content.Load<SpriteFont>("Font");
            _winFont = Content.Load<SpriteFont>("WinFont");
            _boom = Content.Load<SoundEffect>("Boom");  // load in used textures

            Color[] color = new Color[] { _boardColor1 };   // create 1x1 colour list used for textures
            _boardTexture = new Texture2D(GraphicsDevice, 1, 1);    // create new empty 1x1 texture
            _boardTexture.SetData<Color>(color);    // set list as texture's colour data

            color = new Color[] { new Color(205, 210, 106) }; // repeat for second board colour
            _selectedTexture = new Texture2D(GraphicsDevice, 1, 1);
            _selectedTexture.SetData<Color>(color);

            SetUpGame();
        }

        protected override void Update(GameTime gameTime)   
            // method run once per frame, can be run multiple times before draw
        {
            if (_moveHistory.Count > 0)
            {
                if (board.Mate && !_moveHistory.Last().Contains("#"))
                {
                    _moveHistory[_moveHistory.Count - 1] = _moveHistory.Last() + "#";
                }

                else if (board.Check && !_moveHistory.Last().Contains("+") && !_moveHistory.Last().Contains("#"))
                {
                    _moveHistory[_moveHistory.Count - 1] = _moveHistory.Last() + "+";
                }

                else if (board.Draw && !_moveHistory.Last().Contains("1/2 - 1/2"))
                {
                    _moveHistory[_moveHistory.Count - 1] = "1/2 - 1/2";
                }
            }

            if (!board.RequirePromotion) // input only allowed if no piece must promote
            {
                board.Update();

                if (!board.Mate && !board.Draw) // IF game still in play
                {
                    ProcessMouse(); // process mouse input / move logic

                    // update explosion/s
                    foreach (var Explosion in Explosions)   // for each explosion that needs to be updated
                    {
                        Explosion.Update(); // update it
                        if (Explosion.IsDisposed)   // if explosion is finished:
                        { Explosions.Remove(Explosion); break; } // remove from list
                    }
                }
                else // Mate / Stalemate
                {
                    _currentMouseState = Mouse.GetState(); // get mouse data
                    if (winWindow == null)  // if no window popup has been created, create one with the relevant message
                    {
                        if (board.Mate) // someone WON
                        {
                            winWindow = new WinWindow((((_turn != 0) ? "White" : "Black") + " Wins!"),
                                _windowTexture, _winFont, false);
                        }
                        else // stalemate
                        {
                            winWindow = new WinWindow((board.GetDrawType()),
                                _windowTexture, _winFont, true);

                            _boom.Play(); // funny vine boom sound effect plays on stalemate
                        }
                    }

                    if (_currentMouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
                        // "if mouse has just been clicked"
                    {
                        SetUpGame();    // reset the game
                    }
                    _prevMouseState = _currentMouseState;   /* get previous mouse state, used so program knows if mouse 
                                                             is DOWN or if mouse has just been CLICKED */
                }

                if (winWindow != null)  // if window has been initialised
                {
                    winWindow.Update(gameTime); // update window
                }
            }
            else // REQUIRE PROMOTION INPUT
            {
                UpdatePromotion();
            }

            base.Update(gameTime);
        }

        private void UpdatePromotion()  // method to update promotion window
        {
            if (promWindow == null) // if window has not been initialised
            {
                promWindow = new PromotionWindow(_pieceTexture, _turn, _boardTexture); // create new window
            }
            else // window already exists
            {
                _currentMouseState = Mouse.GetState(); // get mouse data
                promWindow.Update(_currentMouseState);  // update window
                if (promWindow.Choice != 4) // if user has selected a piece
                {
                    board.Promote(promWindow.Choice, 1 - _turn);    // tell board to promote the pawn with the selection
                    board.RequirePromotion = false; // game no longer needs promotion input
                    promWindow = null;  // remove promotion window

                    _moveHistory[_moveHistory.Count - 1] = board.prevMove.MoveName;
                }
            }
        }

        private void ProcessMouse() // method to process mouse input when selecting / moving pieces
        {
            _currentMouseState = Mouse.GetState();  // get mouse data from current frame

            if (_currentMouseState.LeftButton == ButtonState.Pressed
                && _prevMouseState.LeftButton == ButtonState.Released)  // "on click"
            {
                int X = (int)_currentMouseState.X / 100;
                int Y = (int)_currentMouseState.Y / 100;    // get x/y positions of mouse on board, 0-8, 0-8

                if (board.selectedSquare != null)   // if you have selected a piece before
                {
                    if (X >= 0 && X < 8 && Y >= 0 && Y < 8) // in bounds check
                    {
                        if (board.Squares[X, Y].ContainsPiece()) // if board contains a piece 
                        {
                            if (board.Squares[X, Y].Piece.Colour == _turn)  // if piece is your colour
                            {
                                TrySelect(X, Y);
                            }    // try to select it

                            else
                            {
                                TryMove(X, Y); // try to move onto that square
                            }
                        }
                        else { TryMove(X, Y); } // if square doesnt contain a piece, just try moving to it
                    }
                }
                else // if you havent selected a piece, try selecting the square you clicked on
                {
                    TrySelect(X, Y);
                }
            }

            _prevMouseState = _currentMouseState;   // set current mouse data as "previous" mouse data used next frame
        }

        private void TrySelect(int X, int Y)    // method to try selecting a piece
        {
            try // it tries
            {
                board.SelectSquare(X, Y, _turn);    // to select a piece
            }
            catch { }
        }

        private void TryMove(int X, int Y)  // method to "try" moving a piece
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
                            board.MovePiece(move); // move enpassant
                            _turn = 1 - _turn;  // switch turn from 1-0 or vice versa
                            _prevMove = move.MoveName;  // get name of move that was just made
                            _moveHistory.Add(_prevMove);    // add to list of moves

                            Explosions.Add(new Explosion(_explosionTexture, // add a little explosion when en passanting
                                move.XFrom + ((move.EnPassantType == 1) ? -1 : 1), move.YFrom));

                            _boom.Play();   // play funny vine boom sound effect

                            break;
                        }
                        else // if move not enpassant
                        {
                            board.MovePiece(move); // move
                            _turn = 1 - _turn;  // switch turn
                            _prevMove = move.MoveName;  // get move name
                            _moveHistory.Add(_prevMove);    // add move name to list
                            break;
                        }
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime) // method to render visuals
        {
            GraphicsDevice.Clear(_boardColor2); // set window colour

            _spriteBatch.Begin();   // begin spritebatch

            // does what it says on the tin tbh
            DrawSquares();
            DrawSelectedPiece();
            DrawCheckSquare();
            DrawPrevMove();
            DrawLegalMoves();
            DrawPieces();
            DrawVariables();
            DrawExplosions();
            DrawWinWindow();
            DrawPromotionWindow();

            _spriteBatch.End(); // end spritebatch, render

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
            _spriteBatch.Draw(_squaresTexture, Vector2.Zero, Color.White); // draw texture of letters/numbers
        }

        private void DrawSelectedPiece()    // method to highlight selected piece
        {
            if (board.selectedSquare != null)   // if a square has been selected
            {
                var x = board.selectedSquare.File; // get file/rank of selected square
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

        public void DrawPrevMove()  // draw green squares on the previous move ala chess.com
        {
            (int XF, int YF) = board.prevSquareFrom;    // get positions of from/to squares of last move
            (int XT, int YT) = board.prevSquareTo;
            _spriteBatch.Draw(_selectedTexture, new Rectangle(XF * 100, YF * 100, 100, 100), Color.White);
            _spriteBatch.Draw(_selectedTexture, new Rectangle(XT * 100, YT * 100, 100, 100), Color.White);
            // draw squares
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

        private void DrawPieces()   // method to draw each piece present on the board
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

            if (board.IsAnimating())
            {
                board.Animate(_spriteBatch);
            }
        }

        private void DrawVariables()
        {

            _spriteBatch.DrawString(_font, "SM: " +
                board.Draw.ToString(), new Vector2(300, 820), Color.White);

            DrawMoveHistory();
        }

        private void DrawMoveHistory() // draws move history
        {
            int W = _graphics.PreferredBackBufferWidth; // get W/H of window
            int H = _graphics.PreferredBackBufferHeight;

            int x = _moveHistory.Count / 66; // 66 moves displayed on each column

            while (W < (1000) + (x * 200))  // if window is too thin to display every move, make it wider
            {
                _graphics.PreferredBackBufferWidth += 200;
                _graphics.ApplyChanges();
                W = _graphics.PreferredBackBufferWidth;
            }

            for (int i = 0; i < _moveHistory.Count; i++)    // for every move in move history
            {
                _spriteBatch.DrawString(_font, _moveHistory[i], new Vector2(
                    810 + ((i / 66) * 200) + ((i % 2) * 90),
                    ((i % 66) / 2) * 30),
                    (i % 2 == 0) ? Color.White : Color.Black);

                // draw the text of the move name, white and black are coloured properly,
                // if text would go too far down, it loops back up and is positioned further to the right
            }
        }

        private void DrawExplosions()   // does exactly what it says on the tin
        {
            foreach (var Explosion in Explosions)
            {
                Explosion.Draw(_spriteBatch);
            }
        }

        private void DrawWinWindow() // draws the popup window when someone wins
        {
            if (winWindow != null)
            {
                winWindow.Draw(_spriteBatch);
            }
        }

        private void DrawPromotionWindow()
        {
            if (promWindow != null)
            {
                promWindow.Draw(_spriteBatch);
            }
        }
    }
}
