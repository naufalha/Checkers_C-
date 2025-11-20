namespace CheckersGameProject
{
   //enums
   public enum PieceType{Pawn,King};
   public enum StatusType{NotStart,Play,Win,Draw};
   public enum PieceColor{Red,Black};

   //struct to store coordinate possition
   public struct Possition
    {
        public int x;
        public int y;
        public Possition(int x,int y)
        {
            this.x=x;
            this.y=y;
        }
    };
}