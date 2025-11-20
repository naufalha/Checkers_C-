CheckersGameProject/
│
├── Contracts/           <-- the interface
│   ├── IBoard.cs
│   ├── ICell.cs
│   ├── IPiece.cs
│   └── IPlayer.cs
│
├── Core/                <-- Tipe data dasar (Building Blocks)
│   ├── Enums.cs         <-- (PieceType, PieceColor, StatusType)
│   └── Position.cs      <-- (Struct Position yang sudah kita perbaiki)
│
├── Models/              <-- Benda nyata (Implementasi)
│   ├── CheckersPiece.cs
│   ├── Cell.cs
│   ├── CheckersBoard.cs
│   └── Player.cs
│
├── GameLogic/           <-- Otak permainan
│   └── CheckersGame.cs  <-- Class raksasa pengatur aturan (TANPA Console.WriteLine)
│
└── Program.cs           <-- Entry Point (UI Console)