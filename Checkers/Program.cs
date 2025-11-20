using System;
using CheckersGameProject.Models;
using CheckersGameProject.Contracts;

namespace CheckersGameProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- CHECKPOINT 1: TESTING BOARD CREATION ---");

            // 1. Coba bikin object Board
            IBoard board = new CheckersBoard();
            Console.WriteLine("Board object created successfully.");

            // 2. Coba visualisasikan papan kosong
            Console.WriteLine("\nVisualisasi Grid (Titik berarti null/kosong):");
            Console.Write("  ");
            for(int i=0; i<8; i++) Console.Write(i + " "); // Header X
            Console.WriteLine();

            for (int y = 0; y < 8; y++)
            {
                Console.Write(y + " "); // Header Y
                for (int x = 0; x < 8; x++)
                {
                    ICell cell = board.Squares[y, x];
                    
                    // Validasi sederhana: Apakah koordinat cell benar?
                    if(cell.Position.X != x || cell.Position.Y != y)
                    {
                        Console.WriteLine($"ERROR: Koordinat salah di [{x},{y}]");
                        return;
                    }

                    // Gambar titik (.) karena belum ada bidak
                    Console.Write(". "); 
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n--- SUKSES: Papan 8x8 terbentuk dengan valid! ---");
            Console.ReadLine();
        }
    }
}