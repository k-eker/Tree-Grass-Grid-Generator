
namespace Supersonic{
    public struct CellData{
        public int X{ get; }
        public int Y{ get; }
        public CellOccupant Occupant{ get; set; }

        public CellData(int x, int y, CellOccupant occupant){
            this.X = x;
            this.Y = y;
            this.Occupant = occupant;
        }
    }
}