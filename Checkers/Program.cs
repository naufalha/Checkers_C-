using System;
using CheckersGameProject.GameLogic;
using CheckersGameProject.Core;
using CheckersGameProject.Contracts;

namespace CheckersGameProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Setup Game
            CheckersGame game = new CheckersGame("Player Black", "Player Red");
            game.StartGame();

            // 2. Game Loop
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CONSOLE CHECKERS (C#) ===");
                PrintBoard(game.Board);

                var currentPlayer = game.GetCurrentPlayer();
                Console.WriteLine($"\nTurn: {currentPlayer.Name} ({currentPlayer.Color})");
                Console.WriteLine("Format: StartX StartY EndX EndY (e.g., '0 2 1 3')");
                Console.WriteLine("Type 'exit' to quit.");
                Console.Write("Enter Move: ");

                string input = Console.ReadLine();
                

                // Exit condition
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit") 
                    break;

                try
                {
                    // 3. Parse Input
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 4)
                    {
                        ShowError("Invalid format! Please enter 4 numbers separated by spaces.");
                        continue;
                    }

                    int x1 = int.Parse(parts[0]);
                    int y1 = int.Parse(parts[1]);
                    int x2 = int.Parse(parts[2]);
                    int y2 = int.Parse(parts[3]);

                    // 4. Execute Move
                    bool success = game.MovePiece(new Position(x1, y1), new Position(x2, y2));

                    if (!success)
                    {
                        ShowError("Invalid Move! Check rules (diagonal, forward, empty target).");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Input Error: {ex.Message}");
                }
            }
        }

        // Helper to show red text for errors
        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n>> {message}");
            Console.ResetColor();
            Console.WriteLine("Press Enter to try again...");
            Console.ReadLine();
        }

        // Visualization Logic
        static void PrintBoard(IBoard board)
        {
            Console.WriteLine("\n   0  1  2  3  4  5  6  7  (X)");
            Console.WriteLine("  ------------------------");

            for (int y = 0; y < 8; y++)
            {
                Console.Write($"{y}|"); // Y-Axis Label
                for (int x = 0; x < 8; x++)
                {
                    var cell = board.Squares[y, x];
                    
                    if (cell.Piece == null)
                    {
                        // Visual style for empty board
                        if ((x + y) % 2 == 0) 
                            Console.Write(" . "); // Light square
                        else 
                            Console.Write("[ ]"); // Dark square (empty)
                    }
                    else
                    {
                        // Render Pieces
                        // Check Black vs Red
                        if (cell.Piece.Color == PieceColor.Black)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan; // Using Cyan for Black visibility
                            Console.Write("[B]");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("[R]");
                        }
                        Console.ResetColor();
                    }
                }
                Console.WriteLine($"|{y}"); // Right side Y-Axis
            }
            Console.WriteLine("  ------------------------");
            Console.WriteLine("   0  1  2  3  4  5  6  7");
        }
    }
}