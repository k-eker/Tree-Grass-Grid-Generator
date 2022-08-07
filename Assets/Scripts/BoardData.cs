
namespace GridGeneration{

    public enum CellOccupant{
        None,
        Grass,
        Tree
    }
    
    public struct BoardData{
        public CellData[,] Data{ get; set; }
    }
}
