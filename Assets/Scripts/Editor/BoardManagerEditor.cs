using UnityEditor;
using UnityEngine;

namespace GridGeneration{
    [CustomEditor(typeof(BoardManager))]
    public class BoardManagerEditor : Editor{
        SerializedProperty boardWidth;
        SerializedProperty boardHeight;

        private const float MAX_GRID_SIZE = 12;

        void OnEnable(){
            boardWidth = serializedObject.FindProperty("boardWidth");
            boardHeight = serializedObject.FindProperty("boardHeight");
        }

        public override void OnInspectorGUI(){
            serializedObject.Update();
            DrawWidthRow();
            DrawHeightRow();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Press R to generate a new board", MessageType.Info);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWidthRow(){
            var rowRect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Board Width");
            Rect r = GUILayoutUtility.GetLastRect();
            r.x += 80;
            r.width -= 140;
            boardWidth.intValue = (int) GUI.HorizontalSlider(r, boardWidth.intValue / 2, 1, MAX_GRID_SIZE / 2) * 2;

            GUI.Label(new Rect(rowRect.width - 20, rowRect.y, 20, rowRect.height), (boardWidth.intValue).ToString());
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHeightRow(){
            var rowRect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Board Height");
            Rect r = GUILayoutUtility.GetLastRect();
            r.x += 80;
            r.width -= 140;
            boardHeight.intValue = (int) GUI.HorizontalSlider(r, boardHeight.intValue / 2, 1, MAX_GRID_SIZE / 2) * 2;

            GUI.Label(new Rect(rowRect.width - 20, rowRect.y, 20, rowRect.height), (boardHeight.intValue).ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}