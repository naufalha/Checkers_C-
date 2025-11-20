using CheckersGameProject.Core;

namespace CheckersGameProject.Contracts
{
    public interface IPlayer
    {
        // Nama pemain (misal: "Player 1")
        string Name { get; set; }

        // Warna pasukan yang dikendalikan pemain ini
        PieceColor Color { get; set; }
    }
}