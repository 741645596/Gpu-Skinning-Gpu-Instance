using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;

namespace IGG.EditorTools
{
    public class MakeCombineMesh : EditorWindow
    {
        private int width = 10;
        private int height = 10;
        [MenuItem("IGG/CombineMeshAni")]
        static void ShowMakeMapWnd()
        {
            EditorUtility.ClearProgressBar();
            MakeCombineMesh wnd = EditorWindow.GetWindow<MakeCombineMesh>("制作CombineMeshAni");
            wnd.minSize = new Vector2(200, 250);
        }



        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 200, 100, 30), "制作CombineMeshAni") == true)
            {
                CreateCombineMesh(6, 6, 3, 3);
                EditorUtility.DisplayDialog("提示", "保存地图数据成功", "确认");
            }
        }



        private void CreateCombineMesh(int w, int h, float xSize, float zSize)
        {
            CombineMeshCreate _creator = new CombineMeshCreate();
            Mesh mesh = _creator.CreateMesh(w, h, xSize, zSize, 2);
            SaveMesh("Assets/AniAssets/CombinMesh/", "combineMesh", mesh);
        }

        // 保存mesh
        public void SaveMesh(string outputFilePath, string meshName, Mesh mesh)
        {
            if (mesh == null)
                return;
            mesh.RecalculateNormals();
            string dataPath = outputFilePath + meshName + ".asset";
            AssetDatabase.CreateAsset(mesh, dataPath);
        }

    }
}
