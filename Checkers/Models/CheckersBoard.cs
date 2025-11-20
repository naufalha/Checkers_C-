using CheckersGameProject.Contracts;
using CheckersGameProject.Core; // Jangan lupa using Core untuk PieceColor

namespace CheckersGameProject.Models
{
    public class CheckersBoard : IBoard
    {
        private ICell[,] _squares;
        public ICell[,] Squares => _squares;

        public CheckersBoard()
        {
            _squares = new Cell[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    _squares[y, x] = new Cell(x, y);

                    // --- LOGIKA BARU: MENARUH BIDAK ---
                    
                    // Rumus Kotak Gelap: (x + y) ganjil
                    if ((x + y) % 2 != 0)
                    {
                        // Baris 0-2: Pasukan Hitam (Black)
                        if (y < 3)
                        {
                            _squares[y, x].Piece = new CheckersPiece(PieceColor.Black, new Position(x, y));
                        }
                        // Baris 5-7: Pasukan Putih (White)
                        else if (y > 4)
                        {
                            _squares[y, x].Piece = new CheckersPiece(PieceColor.Red, new Position(x, y));
                        }
                    }
                }
            }
        }
    }
}