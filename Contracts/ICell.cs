using CheckersGameProject.Core;

namespace CheckersGameProject.Contracts
{
    public interface ICell
    {
        // Kotak ini ada di koordinat mana? (misal: 0,0)
        Position Position { get; set; }

        // Siapa yang berdiri di kotak ini?
        // Tipe datanya IPiece, bukan class konkret.
        // Jika null, berarti kotak kosong.
        IPiece Piece { get; set; }
    }
}