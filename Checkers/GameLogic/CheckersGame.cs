using System;
using System.Collections.Generic;
using System.Linq;
using CheckersGameProject.Contracts;
using CheckersGameProject.Models;
using CheckersGameProject.Core;

namespace CheckersGameProject.GameLogic
{
    public class CheckersGame
    {
        // --- FIELDS (Strictly matching UML) ---
        private IBoard _board;
        private Dictionary<IPlayer, List<IPiece>> _playerData;
        private List<IPlayer> _players;
        private IPlayer _currentPlayer;
        
        // Multi-Jump Logic Fields
        private Position _activePieceInJump; 
        private bool _isInMultipleJump; 
        
        // Game State Fields
        private StatusType _status;
        
        // Draw Logic Fields
        private int _movesWithoutCapture;
        private int _movesWithoutKing; // Tracks moves without King promotion (simplified progress)
        private const int _maxMovesWithoutProgress = 40; // Rule: Draw if 40 moves with no capture/progress

        // --- PUBLIC PROPERTIES (Helpers for UI) ---
        public IBoard Board => _board;
        public IPlayer CurrentPlayer => _currentPlayer;
        public bool IsInDoubleJump => _isInMultipleJump;

        // --- CONSTRUCTOR ---
        public CheckersGame(string player1Name, string player2Name)
        {
            _board = new CheckersBoard();
            
            // Initialize Players: Player 1 is Black (Starts), Player 2 is Red
            _players = new List<IPlayer>
            {
                new Player(player1Name, PieceColor.Black),
                new Player(player2Name, PieceColor.Red)
            };

            _playerData = new Dictionary<IPlayer, List<IPiece>>();
            foreach (var p in _players) _playerData[p] = new List<IPiece>();

            _status = StatusType.NotStart;
        }

        // --- PUBLIC METHODS (UML) ---

        public void StartGame()
        {
            InitializeBoardCells();
            _currentPlayer = _players[0]; // Black starts
            _status = StatusType.Play;
            
            // Reset counters
            _movesWithoutCapture = 0;
            _movesWithoutKing = 0;
            _isInMultipleJump = false;
        }

        public IPlayer GetCurrentPlayer() => _currentPlayer;

        public bool GetPlayerHasPiecesLeft(IPlayer player)
        {
            return _playerData.ContainsKey(player) && _playerData[player].Count > 0;
        }

        public IPlayer? GetWinner()
        {
            if (_status != StatusType.Win) return null;
            
            // Simple logic: If P1 has no pieces, P2 wins, and vice versa.
            if (!GetPlayerHasPiecesLeft(_players[0])) return _players[1];
            if (!GetPlayerHasPiecesLeft(_players[1])) return _players[0];
            return null;
        }

        public bool IsDraw()
        {
            return _status == StatusType.Draw;
        }

        // UML: +FinishGame(showGameStatusCallback: Action<IPlayer>)
        public void FinishGame(Action<IPlayer> showGameStatusCallback)
        {
            IPlayer ?winner = null;

            // Check Win Conditions
            if (!GetPlayerHasPiecesLeft(_players[0]))
            {
                _status = StatusType.Win;
                winner = _players[1];
            }
            else if (!GetPlayerHasPiecesLeft(_players[1]))
            {
                _status = StatusType.Win;
                winner = _players[0];
            }
            // Check Draw Conditions
            else if (_movesWithoutCapture >= _maxMovesWithoutProgress)
            {
                _status = StatusType.Draw;
                winner = null;
            }

            // If game is over, trigger callback
            if (_status != StatusType.Play)
            {
                showGameStatusCallback?.Invoke(winner);
            }
        }

        // Wrapper to allow Program.cs to call the internal MovePiece logic
        public void ExecuteMove(Position from, Position to)
        {
            MovePiece(from, to);
        }

        // --- PRIVATE METHODS (Core Logic) ---

        private void InitializeBoardCells()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    // Place pieces on dark squares ((x+y) is odd)
                    if ((x + y) % 2 != 0)
                    {
                        if (y < 3) // Black (Rows 0-2)
                            AddPieceToPlayerData(_players[0], new CheckersPiece(PieceColor.Black, new Position(x, y)));
                        else if (y > 4) // Red (Rows 5-7)
                            AddPieceToPlayerData(_players[1], new CheckersPiece(PieceColor.Red, new Position(x, y)));
                    }
                }
            }
        }

        private void AddPieceToPlayerData(IPlayer player, IPiece piece)
        {
            _board.Squares[piece.Position.Y, piece.Position.X].Piece = piece;
            _playerData[player].Add(piece);
        }

        // UML: -GetPossibleMovesForPiece(...)
        // Required by architecture, allows for future AI implementation
        private List<Tuple<Position, Position>> GetPossibleMovesForPiece(IPiece piece, bool requireCaptureOnly)
        {
            // Placeholder for AI/Hint logic
            return new List<Tuple<Position, Position>>();
        }

        // UML: -CanMakeaMove(IPlayer)
        private bool CanMakeaMove(IPlayer player)
        {
            // Simplified: As long as they have pieces, we assume they can move (for now)
            return GetPlayerHasPiecesLeft(player);
        }

        // UML: -AnyCaptureExists(IPlayer)
        private bool AnyCaptureExists(IPlayer player)
        {
            foreach (var piece in _playerData[player])
            {
                if (CheckMultipleJump(piece.Position)) return true;
            }
            return false;
        }

        // UML: -MovePiece(from, to)
        private void MovePiece(Position from, Position to)
        {
            if (IsValidMove(from, to, out bool isCapture, out Position capturedPos))
            {
                var sourceCell = _board.Squares[from.Y, from.X];
                var targetCell = _board.Squares[to.Y, to.X];
                var piece = sourceCell.Piece;

                // 1. Move the Piece
                targetCell.Piece = piece;
                sourceCell.Piece = null;
                piece.Position = to;

                bool wasPromoted = false;

                // 2. Handle Capture
                if (isCapture)
                {
                    HandleCapturedPiece(capturedPos);

                    // Check for Double Jump (Multi-jump)
                    if (CheckMultipleJump(to))
                    {
                        // Logic: If promoted to King mid-jump, turn usually ends (Standard Rule)
                        // If not promoted, continue jump
                        wasPromoted = CheckAndPromote(piece);

                        if (!wasPromoted)
                        {
                            _isInMultipleJump = true;
                            _activePieceInJump = to;
                            UpdateDrawCounters(true, false);
                            return; // Do NOT switch player
                        }
                    }
                }

                _isInMultipleJump = false;

                // 3. Handle Promotion (if not already handled in double jump logic)
                if (!wasPromoted)
                {
                    wasPromoted = CheckAndPromote(piece);
                }

                // 4. Update Draw Counters
                UpdateDrawCounters(isCapture, wasPromoted);

                // 5. Switch Player
                SwitchPlayer();
            }
        }

        // UML: -IsValidMove(...) with out parameters
        private bool IsValidMove(Position from, Position to, out bool isCapture, out Position capturedPos)
        {
            isCapture = false;
            capturedPos = new Position(-1, -1);

            if (_status != StatusType.Play) return false;

            // 1. Double Jump Lock: Must use the active piece
            if (_isInMultipleJump && !from.Equals(_activePieceInJump)) return false;

            // 2. Bounds & Null Checks
            if (to.X < 0 || to.X > 7 || to.Y < 0 || to.Y > 7) return false;
            var piece = _board.Squares[from.Y, from.X].Piece;
            
            if (piece == null) return false;
            if (piece.Color != _currentPlayer.Color) return false;
            if (_board.Squares[to.Y, to.X].Piece != null) return false; // Target must be empty

            // 3. Diagonal Check
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;
            if (Math.Abs(dx) != Math.Abs(dy)) return false;

            // 4. Direction Check (King vs Pawn)
            if (piece.TypePiece == PieceType.Pawn)
            {
                // Black moves Down (+Y), Red moves Up (-Y)
                if (piece.Color == PieceColor.Black && dy < 0) return false;
                if (piece.Color == PieceColor.Red && dy > 0) return false;
            }

            // 5. One Step Move (Normal)
            if (Math.Abs(dy) == 1)
            {
                if (_isInMultipleJump) return false; // Cannot do normal move during double jump
                return true;
            }

            // 6. Two Step Move (Capture)
            if (Math.Abs(dy) == 2)
            {
                int midX = from.X + (dx / 2);
                int midY = from.Y + (dy / 2);
                var midPiece = _board.Squares[midY, midX].Piece;

                // Must jump over opponent
                if (midPiece != null && midPiece.Color != piece.Color)
                {
                    isCapture = true;
                    capturedPos = new Position(midX, midY);
                    return true;
                }
            }

            return false;
        }

        // UML: -HandleCapturedPiece(Position)
        private void HandleCapturedPiece(Position capturedPos)
        {
            var cell = _board.Squares[capturedPos.Y, capturedPos.X];
            var piece = cell.Piece;

            if (piece != null)
            {
                var opponent = _players.First(p => p.Color != _currentPlayer.Color);
                if (_playerData.ContainsKey(opponent))
                {
                    _playerData[opponent].Remove(piece);
                }
                cell.Piece = null; // Remove from board
            }
        }

        // UML: -CheckMultipleJump(Position)
        private bool CheckMultipleJump(Position currentPosition)
        {
            // Look ahead 2 squares in all 4 diagonal directions
            int[] dirs = { -2, 2 };
            
            foreach (int dy in dirs)
            {
                foreach (int dx in dirs)
                {
                    Position target = new Position(currentPosition.X + dx, currentPosition.Y + dy);
                    
                    // Reuse IsValidMove logic to check if a capture is physically possible
                    if (IsValidMove(currentPosition, target, out bool isCapture, out _))
                    {
                        if (isCapture) return true;
                    }
                }
            }
            return false;
        }

        // UML: -CheckAndPromote(IPiece)
        private bool CheckAndPromote(IPiece piece)
        {
            if (piece.TypePiece == PieceType.Pawn)
            {
                // Black reaches bottom (7), Red reaches top (0)
                if ((piece.Color == PieceColor.Black && piece.Position.Y == 7) ||
                    (piece.Color == PieceColor.Red && piece.Position.Y == 0))
                {
                    piece.TypePiece = PieceType.King;
                    return true; // Promoted
                }
            }
            return false;
        }

        // UML: -UpdateDrawCounters(bool, bool)
        private void UpdateDrawCounters(bool wasCapture, bool wasKingPromoted)
        {
            if (wasCapture || wasKingPromoted)
            {
                _movesWithoutCapture = 0;
            }
            else
            {
                _movesWithoutCapture++;
            }
        }

        // UML: -SwitchPlayer()
        private void SwitchPlayer()
        {
            // Check Game End Conditions before switching
            FinishGame((winner) =>
            {
                // Callback is handled in Program.cs (UI), but we can log logic here
            });

            if (_status == StatusType.Play)
            {
                _currentPlayer = (_currentPlayer == _players[0]) ? _players[1] : _players[0];
            }
        }
    }
}