using System;
using CheckersGameProject.Models;
using CheckersGameProject.Contracts;

namespace CheckersGameProject
{
    class Program
    {
        static void Main(string[] args)
        {
        
            IBoard board = new CheckersBoard();
            Console.WriteLine("Board object created successfully.");

            // 2. Coba visualisasikan papan kosong
           // ... (kode atas tetap sama)

            Console.WriteLine("Visualisasi Papan dengan Bidak:");
            Console.Write("   ");
            for (int i = 0; i < 8; i++) Console.Write($" {i} ");
            Console.WriteLine();

            for (int y = 0; y < 8; y++)
            {
                Console.Write($" {y} "); // Header Baris
                for (int x = 0; x < 8; x++)
                {
                    ICell cell = board.Squares[y, x];
                    
                    // Jika kosong, gambar titik/kurung
                    if (cell.Piece == null)
                    {
                        // Kotak terang (putih) kita gambar beda biar estetik
                        if((x+y)%2 == 0) Console.Write(" . "); 
                        else Console.Write("[ ]");
                    }
                    else
                    {
                        // Jika ada isinya, cek warnanya
                        if (cell.Piece.Color == CheckersGameProject.Core.PieceColor.Black)
                            Console.Write("[0]"); // B = Black
                        else
                            Console.Write("[O]"); // W = RED
                    }
                }
                Console.WriteLine();
            }

// ... (kode bawah tetap sama)
        

        }
    }
}