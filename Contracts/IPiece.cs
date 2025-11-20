using ChekcersGameProject.Core;
namespace CheckersGameProject.Contracts
{
    public interface IPiece
    {
        PieceColor Color { get; set; }
        Position Position { get; set; }
        PieceType Type { get; set; }
    
    }
}