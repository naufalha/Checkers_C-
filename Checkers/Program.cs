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
            // Black vs Red (American Checkers Standard)
            CheckersGame game = new CheckersGame("Player Black", "Player Red");
            game.StartGame();

            Console.Title = "Console Checkers - American Rules";

            // 2. Main Game Loop
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CHECKERS (C# CONSOLE) ===");
                
                // Draw the board
                PrintBoard(game.Board);

                // Check for Game Over conditions
                if (game.GetWinner() != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nGAME OVER! Winner: {game.GetWinner().Name}");
                    Console.ResetColor();
                    break;
                }
                if (game.IsDraw())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nGAME OVER! It's a DRAW.");
                    Console.ResetColor();
                    break;
                }

                // Show Current Turn Info
                var currentPlayer = game.GetCurrentPlayer();
                Console.WriteLine($"\nTurn: {currentPlayer.Name} ({currentPlayer.Color})");
                
                // Show Double Jump Warning
                if (game.IsInDoubleJump)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(">> DOUBLE JUMP ACTIVE! You must continue capturing with the active piece.");
                    Console.ResetColor();
                }

                Console.WriteLine("Format: StartX StartY EndX EndY (e.g., '2 2 3 3')");
                Console.Write("Enter Move: ");

                string? input = Console.ReadLine();

                // Exit Command
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit") 
                    break;

                try
                {
                    // 3. Parse Input
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    
                    if (parts.Length != 4)
                    {
                        ShowError("Invalid format! Use: x1 y1 x2 y2");
                        continue;
                    }

                    int x1 = int.Parse(parts[0]);
                    int y1 = int.Parse(parts[1]);
                    int x2 = int.Parse(parts[2]);
                    int y2 = int.Parse(parts[3]);

                    // 4. Execute Move
                    // Using ExecuteMove because MovePiece is technically internal logic in the UML
                    // We catch exceptions here if logic fails, though our logic currently returns bool/void
                    game.ExecuteMove(new Position(x1, y1), new Position(x2, y2));
                    
                    // Note: If the move was invalid, the game logic won't change the board/player.
                    // Ideally, ExecuteMove could return a bool to show an error message, 
                    // but based on the UML void signature, we rely on visual feedback (piece didn't move).
                }
                catch (FormatException)
                {
                    ShowError("Please enter numbers only.");
                }
                catch (Exception ex)
                {
                    ShowError($"Error: {ex.Message}");
                }
            }
        }

        // Helper to display red error messages
        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n>> {message}");
            Console.ResetColor();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        // Visualizer for the Board
        static void PrintBoard(IBoard board)
        {
            Console.WriteLine("\n   0  1  2  3  4  5  6  7  (X)");
            Console.WriteLine("  ------------------------");

            for (int y = 0; y < 8; y++)
            {
                Console.Write($"{y}|"); // Left Y-Axis
                for (int x = 0; x < 8; x++)
                {
                    var cell = board.Squares[y, x];
                    
                    // Case 1: Empty Cell
                    if (cell.Piece == null)
                    {
                        // Checkers board pattern: (x+y)%2 == 0 is light, != 0 is dark
                        if ((x + y) % 2 == 0) 
                            Console.Write(" . "); // Light square (unplayable)
                        else 
                            Console.Write("[ ]"); // Dark square (playable but empty)
                    }
                    // Case 2: Occupied Cell
                    else
                    {
                        // Determine Color
                        if (cell.Piece.Color == PieceColor.Black)
                            Console.ForegroundColor = ConsoleColor.Cyan; // Cyan for Black pieces
                        else
                            Console.ForegroundColor = ConsoleColor.Red;  // Red for Red pieces

                        // Determine Symbol (King vs Pawn)
                        string symbol;
                        if (cell.Piece.TypePiece == PieceType.King)
                        {
                            symbol = "K"; // King
                        }
                        else
                        {
                            // Standard Pawn
                            symbol = cell.Piece.Color == PieceColor.Black ? "B" : "R";
                        }

                        Console.Write($"[{symbol}]");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine($"|{y}"); // Right Y-Axis
            }
            Console.WriteLine("  ------------------------");
            Console.WriteLine("   0  1  2  3  4  5  6  7");
        }
    }
}