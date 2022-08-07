using System.Collections.Generic;
using UnityEngine;

namespace Supersonic{
    public class VisualBoard : MonoBehaviour{
        [SerializeField] private GameObject grassPrefab;
        [SerializeField] private GameObject treePrefab;

        private readonly List<GameObject> boardVisuals = new List<GameObject>();

        public void SpawnBoard(BoardData board){
            ClearBoard();
            
            foreach (var cell in board.Data){
                var prefab = cell.Occupant == CellOccupant.Grass ? grassPrefab : treePrefab;
                var obj = Instantiate(prefab, transform);
                obj.transform.position = new Vector2(cell.X, cell.Y);
                boardVisuals.Add(obj);
            }
        }
        
        private void ClearBoard(){
            foreach (var obj in boardVisuals){
                Destroy(obj);
            }
            boardVisuals.Clear();
        }
    }
}