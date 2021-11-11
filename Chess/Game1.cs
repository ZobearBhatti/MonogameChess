using Chess.Pieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Chess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Variables

        private Color _boardColor1 = new Color(240, 217, 181);
        private Color _boardColor2 = new Color(181, 136, 99);
        private Texture2D _boardTexture;
        private Texture2D _pieceTexture;
        private Texture2D _selectedTexture;
        private Texture2D _dotTexture;
        private Texture2D _circleTexture;
        private Texture2D _squaresTexture;
        private SpriteFont _font;

        public Piece[,] Board { get; set; }

        private int _turn;
        private bool _pieceSelected;
        private Vector2 _selectedPiece;
        private byte _selectedPieceType;
        private byte _selectedPieceColour;  // kinda using bytes to save on mem not much tho thats effort
        private Vector2 _prevMoveFrom;
        private Vector2 _prevMoveTo;
        private byte _prevPiece;

        private List<Vector2> _legalMoves;
        private int[,] _attackMap;

        private MouseState _prevMouseState;
        private MouseState _currentMouseState;

        private byte _promotionPiece;

        private bool _isCheck;
        private bool _isCheckmate;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 900;
            _graphics.PreferredBackBufferHeight = 900;
        }

        protected override void Initialize()
        {
            Color[] color = new Color[] { _boardColor1 };
            _boardTexture = new Texture2D(GraphicsDevice, 1, 1);
            _boardTexture.SetData<Color>(color);

            color = new Color[] { new Color(205, 210, 106) };
            _selectedTexture = new Texture2D(GraphicsDevice, 1, 1);
            _selectedTexture.SetData<Color>(color);

            _promotionPiece = 1; // default promote to queen
            _turn = 0;
            _pieceSelected = false;
            _prevMouseState = new MouseState();
            _selectedPiece = new Vector2(-1, -1);
            _prevMoveFrom = new Vector2(-1, -1);
            _prevMoveTo = new Vector2(-1, -1);
            _prevPiece = 6;
            _legalMoves = new List<Vector2>();
            _isCheck = false;
            _isCheckmate = false;
            _attackMap = new int[8,8];

            base.Initialize();
        }

        private void SetUpBoard()
        {
            Board = new Piece[8, 8];
            //setup pawns
            for (int i = 0; i < 8; i++)
            { Board[i, 1] = new Pawn(_pieceTexture, 1);
                Board[i, 6] = new Pawn(_pieceTexture, 0); }

            //setup rooks
            {
                Board[0, 0] = new Rook(_pieceTexture, 1); Board[7, 0] = new Rook(_pieceTexture, 1);
                Board[0, 7] = new Rook(_pieceTexture, 0); Board[7, 7] = new Rook(_pieceTexture, 0);
            }

            //setup knights
            {
                Board[1, 0] = new Knight(_pieceTexture, 1); Board[6, 0] = new Knight(_pieceTexture, 1);
                Board[1, 7] = new Knight(_pieceTexture, 0); Board[6, 7] = new Knight(_pieceTexture, 0);
            }

            //setup bishops
            {
                Board[2, 0] = new Bishop(_pieceTexture, 1); Board[5, 0] = new Bishop(_pieceTexture, 1);
                Board[2, 7] = new Bishop(_pieceTexture, 0); Board[5, 7] = new Bishop(_pieceTexture, 0);
            }

            //setup queens and kings
            {
                Board[3, 0] = new Queen(_pieceTexture, 1); Board[4, 0] = new King(_pieceTexture, 1);
                Board[3, 7] = new Queen(_pieceTexture, 0); Board[4, 7] = new King(_pieceTexture, 0);
            }
        } // default piece positions

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pieceTexture = Content.Load<Texture2D>("Pieces");
            _dotTexture = Content.Load<Texture2D>("Dot");
            _circleTexture = Content.Load<Texture2D>("Circle");
            _squaresTexture = Content.Load<Texture2D>("Squares");
            _font = Content.Load<SpriteFont>("Font");
            SetUpBoard();
            GiveEachPieceItsPosition();
        }

        private void GiveEachPieceItsPosition() // does what it says on the tin
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Board[x, y] is Piece)
                    {
                        Board[x, y].x = x;
                        Board[x, y].y = y;
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();
            int _boardX = (int)_currentMouseState.X / 100;
            int _boardY = (int)_currentMouseState.Y / 100;

            if (_currentMouseState.LeftButton == ButtonState.Pressed 
                && _prevMouseState.LeftButton == ButtonState.Released)
            {
                if (!(_boardX > 7 || _boardY > 7 || _boardX < 0 || _boardY < 0))    // if not out of bounds
                {
                    ClickBoard(_boardX, _boardY);
                }
                else if (_boardX == 8 && _boardY < 6 && _boardY > 1)
                {
                    _promotionPiece = (byte)(_boardY - 1);
                }
            }
            _prevMouseState = _currentMouseState;
            base.Update(gameTime);
        }

        private void GenerateAttacks(int attackColour)  // generate every square that can be attacked
        {
            foreach (var piece in Board)    // for each piece on the board
            {
                if (piece is Piece) // if it is a piece
                {
                    if (piece.Colour == attackColour)   // if it is an opp
                    {
                        foreach (Vector2 pos in piece.GenerateLegalMoves(Board, true))  
                        {
                            _attackMap[(int)pos.X, (int)pos.Y] = 1; // add where it can attack to attackmap
                        }
                    }
                }
            }
        }

        private void ClickBoard(int _boardX, int _boardY)
        {
            if (Board[_boardX, _boardY] is Piece)   // if selecting piece
            {
                if (Board[_boardX, _boardY].Colour == _turn)   // if selecting OWN piece 
                {
                    _pieceSelected = true; // you have selected a piece
                    _selectedPiece = new Vector2(_boardX, _boardY); // get position of selected piece
                    _selectedPieceColour = (byte)_turn;  // get colour of piece
                    _selectedPieceType = Board[_boardX, _boardY].Type;  //  get piece type
                    _legalMoves = Board[_boardX, _boardY].GenerateLegalMoves(Board, false); //gen legal moves
                    AdjustForCheck();
                }
                else if (_pieceSelected && _legalMoves.Contains(new Vector2(_boardX, _boardY))) // take a piece
                {
                    MovePiece(_boardX, _boardY);
                    AfterPieceMove();
                }
            }
            else // if selecting empty space
            {
                if (_pieceSelected && _legalMoves.Contains(new Vector2(_boardX, _boardY))) // move
                {
                    MovePiece(_boardX, _boardY);
                    AfterPieceMove();
                }
            }
        }

        private void AdjustForCheck()
        {
            if (_selectedPieceType == 0) // if king
            {
                foreach (Vector2 pos in _legalMoves.ToArray())
                {
                    if (_attackMap[(int)pos.X, (int)pos.Y] == 1)
                    {
                        _legalMoves.Remove(pos);
                    }
                }
            }
        }

        private void MovePiece(int _boardX, int _boardY)
        {
            var temp = Board[(int)_selectedPiece.X, (int)_selectedPiece.Y]; // get moving piece
            if (temp.Type == 5 && (_boardY == 0 || _boardY == 7))   // if promotion
            {
                switch (_promotionPiece)    // switch according to selected promotion piece
                {
                    case 1:
                        Board[_boardX, _boardY] = new Queen(_pieceTexture, temp.Colour); break;
                    case 2:
                        Board[_boardX, _boardY] = new Bishop(_pieceTexture, temp.Colour); break;
                    case 3:
                        Board[_boardX, _boardY] = new Knight(_pieceTexture, temp.Colour); break;
                    case 4:
                        Board[_boardX, _boardY] = new Rook(_pieceTexture, temp.Colour); break;
                }
            }
            else // if not promoting, use stored piece as replacement
            {
                Board[_boardX, _boardY] = temp; // replace piece
            }
            _prevPiece = Board[(int)_selectedPiece.X, (int)_selectedPiece.Y].Type; // get old piece type
            _prevMoveFrom = _selectedPiece; // get move origin
            _prevMoveTo = new Vector2(_boardX, _boardY); // get move destination

            EnPassant(_boardX, _boardY);
            Board[(int)_selectedPiece.X, (int)_selectedPiece.Y] = null; // delete old piece
            PawnLogic(_boardX, _boardY);

            foreach (Vector2 pos in Board[_boardX, _boardY].GenerateLegalMoves(Board, false))
            {
                if (Board[(int)pos.X,(int)pos.Y] is King)
                { _isCheck = true; break; }
            }
            Board[_boardX, _boardY].x = _boardX;
            Board[_boardX, _boardY].y = _boardY;
        }

        private void EnPassant(int _boardX, int _boardY)
        {
            if (_prevPiece == 5 && _prevMoveFrom.X != _prevMoveTo.X) // if pawn moved diag and didnt take:
            {
                if (Board[(int)_prevMoveTo.X, (int)_prevMoveTo.Y].Colour == 0) // White
                {
                    if (Board[(int)_prevMoveTo.X, (int)_prevMoveTo.Y + 1] is Pawn &&
                        Board[(int)_prevMoveTo.X, (int)_prevMoveTo.Y + 1]._canBeEnPassant)  // if piece below is en passant pawn
                    {
                        Board[_boardX, _boardY + 1] = null; // delete piece below
                    }
                }
                else // black
                {
                    if (Board[(int)_prevMoveTo.X, (int)_prevMoveTo.Y - 1] is Pawn &&
                    Board[(int)_prevMoveTo.X, (int)_prevMoveTo.Y - 1]._canBeEnPassant)  // if piece below is en passant pawn
                    {
                        Board[_boardX, _boardY - 1] = null; // delete piece above
                    }
                }
            }
        }

        private void PawnLogic(int x, int y)
        {
            foreach (Piece piece in Board)  // clear EnPassant for each pawn
            {
                if (piece is Pawn)
                {
                    piece._canBeEnPassant = false; // after a move, pawns that COULD be en passanted but havent been can now no longer be
                }
            }

            if (_prevPiece == 5 && ((_prevMoveFrom.Y == 1 && _prevMoveTo.Y == 3) || (_prevMoveFrom.Y == 6 && _prevMoveTo.Y == 4)))  // if piece is pawn jumping 2
            {
                Board[x, y]._canBeEnPassant = true; // pawn can be en passanted
            }
        }

        private void AfterPieceMove()
        {
            _turn = 1 - _turn;  // change turn
            _pieceSelected = false; // piece NOT selected
            _selectedPiece = new Vector2(-1, -1);   // no piece selected
            _attackMap = new int[8, 8]; // clear attack map
            GenerateAttacks(1 - _turn);
            _legalMoves.Clear(); // clear legal moves
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_boardColor2);

            _spriteBatch.Begin();

            // Draw board grid
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if ((x+y) % 2 == 0)
                    {
                        _spriteBatch.Draw(_boardTexture, new Vector2(x * 100, y * 100),
                            new Rectangle(0, 0, 100, 100), Color.White);
                    }
                }
            }

            // Highlight selected piece and previous move
            _spriteBatch.Draw(_selectedTexture, new Rectangle((int)_selectedPiece.X * 100, (int)_selectedPiece.Y * 100, 100, 100),
                Color.DarkOliveGreen);

            _spriteBatch.Draw(_selectedTexture, new Rectangle((int)_prevMoveFrom.X * 100, (int)_prevMoveFrom.Y * 100, 100, 100),
            Color.White);

            _spriteBatch.Draw(_selectedTexture, new Rectangle((int)_prevMoveTo.X * 100, (int)_prevMoveTo.Y * 100, 100, 100),
            Color.White);

            //      highlight attack map
            //for (int x = 0; x < 8; x++)
            //{
            //    for (int y = 0; y < 8; y++)
            //    {
            //        if (_attackMap[x, y] == 1)
            //        {
            //            _spriteBatch.Draw(_selectedTexture, new Rectangle(x * 100, y * 100, 100, 100),
            //            Color.Red * 0.8f);
            //        }
            //    }
            //}

            // Draw Pieces
            foreach (var piece in Board)
            {
                if (piece is Piece)
                {
                    piece.Draw(_spriteBatch);
                }
            }

            // Draw Dot or Circle on each legal move
            foreach (Vector2 pos in _legalMoves)
            {
                if (Board[(int)pos.X, (int)pos.Y] is Piece)
                {
                    _spriteBatch.Draw(_circleTexture, new Rectangle((int)pos.X * 100, (int)pos.Y * 100, 100, 100), new Rectangle(0, 0, 200, 200), Color.White * 0.4f);
                }
                else
                {
                    _spriteBatch.Draw(_dotTexture, new Rectangle((int)pos.X * 100, (int)pos.Y * 100, 100, 100), new Rectangle(0, 0, 200, 200), Color.White * 0.4f);
                }

            }

            // Draw Highlight of selected promotion piece
            _spriteBatch.Draw(_selectedTexture, new Rectangle(800, 100 + (_promotionPiece * 100), 100, 100),
                    new Rectangle(_promotionPiece * 200, _turn * 200, 200, 200), Color.White);

            // Draw Promotion Pieces
            for (int i = 1; i <= 4; i++)
            {
                _spriteBatch.Draw(_pieceTexture, new Rectangle(800, 100 + (i * 100), 100, 100),
                    new Rectangle(i * 200, _turn * 200, 200, 200), Color.White);
            }

            // draw square names
            _spriteBatch.Draw(_squaresTexture, Vector2.Zero, Color.White*0.8f);

            // Print Variables
            {
                _spriteBatch.DrawString(_font, "pieceSelected: " + _pieceSelected.ToString(), new Vector2(10, 800), Color.White);
                _spriteBatch.DrawString(_font, "turn: " + _turn.ToString(), new Vector2(10, 820), Color.White);
                _spriteBatch.DrawString(_font, "selectedPiece: " + _selectedPiece.ToString(), new Vector2(10, 840), Color.White);
                _spriteBatch.DrawString(_font, "selectedPieceColour: " + _selectedPieceColour.ToString(), new Vector2(10, 860), Color.White);
                _spriteBatch.DrawString(_font, "selectedPieceType: " + _selectedPieceType.ToString(), new Vector2(10, 880), Color.White);
                _spriteBatch.DrawString(_font, "prevMoveFrom: " + _prevMoveFrom.ToString(), new Vector2(200, 800), Color.White);
                _spriteBatch.DrawString(_font, "prevMoveTo: " + _prevMoveTo.ToString(), new Vector2(200, 820), Color.White);
                _spriteBatch.DrawString(_font, "prevPiece: " + _prevPiece.ToString(), new Vector2(200, 840), Color.White);
                _spriteBatch.DrawString(_font, "check: " + _isCheck.ToString(), new Vector2(200, 860), Color.White);
                _spriteBatch.DrawString(_font, "checkmate: " + _isCheckmate.ToString(), new Vector2(200, 880), Color.White);

                if (_selectedPiece != new Vector2(-1, -1))
                {
                    _spriteBatch.DrawString(_font, "x: " + Board[(int)_selectedPiece.X, (int)_selectedPiece.Y].x
                        .ToString(), new Vector2(400, 800), Color.White);
                    _spriteBatch.DrawString(_font, "y: " + Board[(int)_selectedPiece.X, (int)_selectedPiece.Y].y
                        .ToString(), new Vector2(400, 820), Color.White);
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
