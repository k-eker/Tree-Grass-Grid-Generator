using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace GridGeneration{
    [RequireComponent(typeof(VisualBoard))]
    public class BoardManager : MonoBehaviour{
        [SerializeField] private int boardWidth = 6;
        [SerializeField] private int boardHeight = 8;

        private VisualBoard visualBoard;

        private const int MAX_REPETITION = 2;

        private void Start(){
            visualBoard = GetComponent<VisualBoard>();
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
            var stopwatch = new Stopwatch();
 
            stopwatch.Start();
            var newBoard = GenerateBoardData(boardWidth, boardHeight);
            visualBoard.SpawnBoard(newBoard);
            stopwatch.Stop();

            Debug.Log("Board generated. Elapsed time : " + stopwatch.ElapsedMilliseconds + " ms");
        }

        private BoardData GenerateBoardData(int width, int height){
            if (width % 2 != 0 || height % 2 != 0){
                throw new Exception("Board width and height must be even");
            }

            var board = new BoardData{
                Data = new CellData[width, height]
            };

            do{
                for (int x = 0; x < width; x++){
                    for (int y = 0; y < height; y++){
                        var occupant = PickCellOccupant(x, y, board.Data);
                        board.Data[x, y] = new CellData(x, y, occupant);
                    }
                }
            } 
            while (!EnsureValidBoard(board.Data));
            
            return board;
        }

        private CellOccupant PickCellOccupant(int x, int y, CellData[,] boardData){
            
            if (CheckForHorizontalRepetition(x, y, boardData, out CellOccupant suggestedHorizontal)){
                return suggestedHorizontal;
            }
            if (CheckForVerticalRepetition(x, y, boardData, out CellOccupant suggestedVertical)){
                return suggestedVertical;
            }
            if (EnsureHorizontalCount(x, y, boardData, out CellOccupant suggestedOccupantH)){
                return suggestedOccupantH;
            }
            if (EnsureVerticalCount(x, y, boardData, out CellOccupant suggestedOccupantV)){
                return suggestedOccupantV;
            }
            
            return Random.value < 0.5f ? CellOccupant.Grass : CellOccupant.Tree;
        }

        private bool EnsureValidBoard(CellData[,] boardData){
            for (int x = 0; x < boardWidth; x++){
                var columnTreeCount = GetColumnTreeCount(x, boardData);
                if (columnTreeCount != boardHeight / 2){
                    return false;
                }

                for (int y = 0; y < boardHeight; y++){
                    var rowTreeCount = GetRowTreeCount(y, boardData);
                    if (rowTreeCount != boardWidth / 2){
                        return false;
                    }
                }
            }

            return true;
        }

        private int GetRowTreeCount(int y, CellData[,] boardData){
            var count = 0;
            for (int i = 0; i < boardWidth; i++){
                if (boardData[i, y].Occupant == CellOccupant.Tree){
                    count++;
                }
            }

            return count;
        }

        private int GetColumnTreeCount(int x, CellData[,] boardData){
            var count = 0;
            for (int i = 0; i < boardHeight; i++){
                if (boardData[x, i].Occupant == CellOccupant.Tree){
                    count++;
                }
            }

            return count;
        }

        private bool CheckForHorizontalRepetition(int x, int y, CellData[,] boardData,
            out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;

            if (x < MAX_REPETITION){
                return false;
            }

            var prevOccupant = CellOccupant.None;
            var repetitionFound = true;

            for (int i = x - 1; i > x - MAX_REPETITION - 1; i--){
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

        private bool CheckForVerticalRepetition(int x, int y, CellData[,] boardData, out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;

            if (y < MAX_REPETITION){
                return false;
            }

            var prevOccupant = CellOccupant.None;
            var repetitionFound = true;

            for (int i = y - 1; i > y - MAX_REPETITION - 1; i--){
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

            var allowedCount = boardWidth / 2;
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

            if (treeCount >= allowedCount){
                suggestedOccupant = CellOccupant.Grass;
                return true;
            }

            if (grassCount >= allowedCount){
                suggestedOccupant = CellOccupant.Tree;
                return true;
            }

            return false;
        }

        private bool EnsureVerticalCount(int x, int y, CellData[,] boardData, out CellOccupant suggestedOccupant){
            suggestedOccupant = CellOccupant.None;

            var allowedCount = boardHeight / 2;
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

            if (treeCount >= allowedCount){
                suggestedOccupant = CellOccupant.Grass;
                return true;
            }

            if (grassCount >= allowedCount){
                suggestedOccupant = CellOccupant.Tree;
                return true;
            }

            return false;
        }
    }
}