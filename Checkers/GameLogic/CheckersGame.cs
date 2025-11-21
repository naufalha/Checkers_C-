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
        private IBoard _board;
        private Dictionary<IPlayer, List<IPiece>> _playerData;
        private List<IPlayer> _players;
        private IPlayer _currentPlayer;
        private StatusType _status;

        public IBoard Board => _board;
        public IPlayer CurrentPlayer => _currentPlayer;

        public CheckersGame(string player1Name, string player2Name)
        {
            _board = new CheckersBoard();
            _players = new List<IPlayer>
            {
                new Player(player1Name, PieceColor.Black),
                new Player(player2Name, PieceColor.Red) // Menggunakan Red
            };

            _playerData = new Dictionary<IPlayer, List<IPiece>>();
            foreach (var p in _players) _playerData[p] = new List<IPiece>();

            _status = StatusType.NotStart;
        }

        public void StartGame()
        {
            InitializeBoardCells();
            _currentPlayer = _players[0]; // Black (Atas) jalan duluan
            _status = StatusType.Play;
        }

        private void InitializeBoardCells()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 != 0)
                    {
                        if (y < 3) // Black di Atas
                        {
                            var piece = new CheckersPiece(PieceColor.Black, new Position(x, y));
                            AddPieceToPlayerData(_players[0], piece);
                        }
                        else if (y > 4) // Red di Bawah
                        {
                            var piece = new CheckersPiece(PieceColor.Red, new Position(x, y)); // Ganti White -> Red
                            AddPieceToPlayerData(_players[1], piece);
                        }
                    }
                }
            }
        }

        private void AddPieceToPlayerData(IPlayer player, IPiece piece)
        {
            _board.Squares[piece.Position.Y, piece.Position.X].Piece = piece;
            _playerData[player].Add(piece);
        }

        // --- LOGIKA GERAK (Basic Movement) ---

        public bool MovePiece(Position from, Position to)
        {
            if (!IsValidMove(from, to, out bool isCapture, out Position capturedPos)) 
                return false;/// 1. Cek Validasi
            //i (!IsValidMove(from, to)) return false;

            // 2. Ambil Referensi
            var sourceCell = _board.Squares[from.Y, from.X];
            var targetCell = _board.Squares[to.Y, to.X];
            var piece = sourceCell.Piece;

            // 3. Pindahkan Bidak
            targetCell.Piece = piece;   // Taruh di tujuan
            sourceCell.Piece = null;    // Hapus dari asal
            piece.Position = to;        // Update koordinat internal bidak

            //gerakan makan
            if (isCapture)
            {
                HandleCapturedPiece(capturedPos);
            }
            // 4. Ganti Giliran
            SwitchPlayer();
            return true;
        }

        private bool IsValidMove(Position from, Position to)
        {
            // A. Cek Bounds (0-7)
            if (to.X < 0 || to.X > 7 || to.Y < 0 || to.Y > 7) return false;

            var piece = _board.Squares[from.Y, from.X].Piece;
            
            // B. Validasi Dasar
            if (piece == null) return false; // Asal kosong
            if (piece.Color != _currentPlayer.Color) return false; // Bukan giliranmu
            if (_board.Squares[to.Y, to.X].Piece != null) return false; // Tujuan harus kosong

            // C. Cek Diagonal
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;
            if (Math.Abs(dx) != Math.Abs(dy)) return false; // Harus diagonal

            // D. Cek Arah (Pion hanya boleh maju)
            // Black (Y=0 ke Y=7) -> Harus nambah (+)
            if (piece.Color == PieceColor.Black && dy < 0) return false;
            // Red (Y=7 ke Y=0) -> Harus kurang (-)
            if (piece.Color == PieceColor.Red && dy > 0) return false;

            // E. Jarak (Untuk sekarang hanya boleh 1 langkah)
            // Nanti kita ubah ini saat implementasi "Makan/Capture"
            if (Math.Abs(dy) == 1) return true;

            return false;
        }

        private void SwitchPlayer()
        {
            _currentPlayer = (_currentPlayer == _players[0]) ? _players[1] : _players[0];
        }

        public IPlayer GetCurrentPlayer() => _currentPlayer;
    }
}