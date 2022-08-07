using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Supersonic{
    public class BoardManager : MonoBehaviour{
        [SerializeField] private int boardWidth = 6;
        [SerializeField] private int boardHeight = 8;
        [SerializeField] private int maxRepetition = 2;

        [SerializeField] private VisualBoard visualBoard;

        private void Start(){
            Generate();
        }
#if UNITY_EDITOR
        private void Update(){
            if (Input.GetKeyDown(KeyCode.R)){
                Generate();
            }
        }
#endif

        private void Generate(){
            var newBoard = GenerateBoardData(boardWidth, boardHeight);
            visualBoard.SpawnBoard(newBoard);
        }

        private BoardData GenerateBoardData(int width, int height){
            if (width % 2 != 0 || height % 2 != 0){
                throw new Exception("Board width and height must be even");
            }

            var board = new BoardData{
                Data = new CellData[width, height]
            };

            for (int x = 0; x < width; x++){
                for (int y = 0; y < height; y++){
                    var rowCountValid = EnsureHorizontalCount(x, y, board.Data, out CellOccupant suggestedOccupantH);
                    var columnCountValid = EnsureVerticalCount(x, y, board.Data, out CellOccupant suggestedOccupantV);
                    
                    var repetitionFoundHorizontal = EnsureHorizontalRepetition(x, y, board.Data, out CellOccupant suggestedHorizontal);
                    var repetitionFoundVertical = EnsureVerticalRepetition(x, y, board.Data, out CellOccupant suggestedVertical);
                    if (repetitionFoundHorizontal && repetitionFoundVertical && suggestedHorizontal != suggestedVertical){
                        x = 0;
                        continue;
                    }
                    
                    
                    if (repetitionFoundHorizontal){
                        board.Data[x, y] = new CellData(x, y, suggestedHorizontal);
                    }
                    else if (repetitionFoundVertical){
                        board.Data[x, y] = new CellData(x, y, suggestedVertical);
                    }
                    else if (rowCountValid){
                        board.Data[x, y] = new CellData(x, y, suggestedOccupantH);
                    }
                    else if (columnCountValid){
                        board.Data[x, y] = new CellData(x, y, suggestedOccupantV);
                    }
                    else{
                        var occupant = Random.value < 0.5f ? CellOccupant.Grass : CellOccupant.Tree;
                        board.Data[x, y] = new CellData(x, y, occupant);
                    }
                }
                
            }

            EnsureReplaced(board.Data);
            
            return board;
        }

        private void EnsureReplaced(CellData[,] boardData){
            for (int x = 0; x < boardWidth; x++){
                var columnTreeCount = GetColumnTreeCount(x, boardData);
                if (columnTreeCount != boardHeight / 2){
                    Debug.Log("COLUMN COUNT : " + x + " : " +  columnTreeCount);
                }
                for (int y = 0; y < boardHeight; y++){
                    var rowTreeCount = GetRowTreeCount(y, boardData);
                    if (rowTreeCount != boardWidth / 2){
                        Debug.Log("ROW COUNT : " + y + " : " + rowTreeCount);
                    }
                }
            }
        }

        private int GetRowTreeCount(int y, CellData[,] boardData){
            var count = 0;
            for (int i = 0; i < boardWidth; i++){
                if (boardData[i,y].Occupant == CellOccupant.Tree){
                    count++;
                }
            }

            return count;
        }
        private int GetColumnTreeCount(int x, CellData[,] boardData){
            var count = 0;
            for (int i = 0; i < boardHeight; i++){
                if (boardData[x,i].Occupant == CellOccupant.Tree){
                    count++;
                }
            }

            return count;
        }

        private bool EnsureHorizontalRepetition(int x, int y, CellData[,] boardData, out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;

            if (x < maxRepetition){
                return false;
            }

            var prevOccupant = CellOccupant.None;
            var repetitionFound = true;

            for (int i = x - 1; i > x - maxRepetition - 1; i--){
                var cell = boardData[i, y];
                if (prevOccupant != CellOccupant.None && cell.Occupant != prevOccupant){
                    repetitionFound = false;
                }

                prevOccupant = cell.Occupant;
            }

            if (repetitionFound){
                suggestedOccupant = prevOccupant == CellOccupant.Tree ? CellOccupant.Grass : CellOccupant.Tree;
                return true;
            }

            return false;
        }
        
        private bool EnsureVerticalRepetition(int x, int y, CellData[,] boardData, out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;
            
            if (y < maxRepetition){
                return false;
            }

            var prevOccupant = CellOccupant.None;
            var repetitionFound = true;

            for (int i = y - 1; i > y - maxRepetition - 1; i--){
                var cell = boardData[x, i];
                if (prevOccupant != CellOccupant.None && cell.Occupant != prevOccupant){
                    repetitionFound = false;
                }

                prevOccupant = cell.Occupant;
            }

            if (repetitionFound){
                suggestedOccupant = prevOccupant == CellOccupant.Tree ? CellOccupant.Grass : CellOccupant.Tree;
                return true;
            }

            return false;
        }

        private bool EnsureHorizontalCount(int x, int y, CellData[,] boardData, out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;
            
            var xBudget = boardWidth / 2;
            var treeCount = 0;
            var grassCount = 0;
            for (int i = 0; i < x; i++){
                var cell = boardData[i, y];
                if (cell.Occupant == CellOccupant.Tree){
                    treeCount++;
                }
                else{
                    grassCount++;
                }
            }
            
            if (treeCount >= xBudget){
                suggestedOccupant = CellOccupant.Grass;
                return true;
            }
            
            if (grassCount >= xBudget){
                suggestedOccupant = CellOccupant.Tree;
                return true;
            }

            return false;
        }
        
        private bool EnsureVerticalCount(int x, int y, CellData[,] boardData, out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;
            
            var yBudget = boardHeight / 2;
            var treeCount = 0;
            var grassCount = 0;
            for (int i = 0; i < y; i++){
                var cell = boardData[x, i];
                if (cell.Occupant == CellOccupant.Tree){
                    treeCount++;
                }
                else{
                    grassCount++;
                }
            }
            
            if (treeCount >= yBudget){
                suggestedOccupant = CellOccupant.Grass;
                return true;
            }
            
            if (grassCount >= yBudget){
                suggestedOccupant = CellOccupant.Tree;
                return true;
            }

            return false;
        }
    }
}