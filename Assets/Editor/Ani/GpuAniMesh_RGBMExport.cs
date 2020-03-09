using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class GpuAniMesh_RGBMExport : IAniMeshExport {


	private MeshAnimation m_AniMesh = null;
	// 设置mesh ani 数据
	public void SetAniMeshData(MeshAnimation aniMesh)
	{
		m_AniMesh = aniMesh;
	}


	public void Export(string outputFilePath,Quaternion quaternion, string AniMeshName)
	{
		string filePath = outputFilePath + "/GpuAniMeshRGBM/";
		SaveMeshTexture(filePath, AniMeshName);
		SaveMesh(filePath, AniMeshName);
		SaveMaterial(filePath, AniMeshName);
		SavePrefab (filePath,quaternion, AniMeshName);
		EditorUtility.DisplayDialog("Tip", "Cpu Mesh Animation Export Complete" + m_AniMesh.m_ActorName, "OK");
		AssetDatabase.Refresh();
	}


	// 保存顶点动画纹理
	public void SaveMeshTexture(string outputFilePath, string AniMeshName)
	{
		int totalFrame = m_AniMesh.GetTotalFrame();


		int totalvectors = m_AniMesh.GetTotalVertices();
		int npower = IGGUtil.GettoPower(totalvectors);
		Texture2D combinedTex = new Texture2D(npower, totalFrame, TextureFormat.RGBA32, false);
		combinedTex.filterMode = FilterMode.Point;
		combinedTex.wrapMode = TextureWrapMode.Clamp;
		combinedTex.anisoLevel = 0;
		//
		int vertexCount = 0;
		int SubMeshVertexNum = totalvectors;
		for (int frame = 0; frame < totalFrame; frame++) 
		{
			for(int k = 0; k < SubMeshVertexNum; k ++)
			{
				Vector3 vertex = m_AniMesh.m_Mesh.m_AniMesh[frame][k];
				Color c = IGGUtil.EncodeRGBM(vertex);
				combinedTex.SetPixel(k,  frame, c);
			}
		}
		combinedTex.Apply(false);
		string dataPath = outputFilePath +  AniMeshName + "_AT.asset";
		AssetDatabase.CreateAsset(combinedTex, dataPath);
	}
	// 保存mesh
	public void SaveMesh(string outputFilePath, string MeshName)
	{
		Mesh mesh = new Mesh();

		int totalvectors = m_AniMesh.m_Mesh.m_vertices.Length;
		int npower = IGGUtil.GettoPower(totalvectors);

		Vector2[] uv = m_AniMesh.m_Mesh.m_UV;
		Vector3[] posArray = new Vector3[totalvectors];
		for (int i = 0; i < posArray.Length; i++) {
			posArray[i].x = uv[i].x;
			posArray[i].y = uv[i].y;
			posArray[i].z = i * 1.0f / npower;
		}
		mesh.vertices = posArray;

		mesh.triangles = m_AniMesh.m_Mesh.m_Triangles;

		mesh.RecalculateNormals();

		string dataPath = outputFilePath +  MeshName + "_M.asset";
		AssetDatabase.CreateAsset(mesh, dataPath);
	}

	// 保存mat文件
	public void SaveMaterial(string outputFilePath, string MaterialName)
	{
		Shader s = Shader.Find("GpuCustomUnlitRGBM");
		Material mat = new Material(s);
		mat.name = MaterialName;
		mat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath("Assets/Models/Texture/" + MaterialName + ".tga", typeof(Texture2D)) as Texture2D);
		mat.SetTexture("_AniMeshTex", AssetDatabase.LoadAssetAtPath(outputFilePath + MaterialName + "_AT.asset", typeof(Texture2D)) as Texture2D);
		mat.SetColor("_GroupColor", Color.green);
		AssetDatabase.CreateAsset(mat, outputFilePath + MaterialName + ".mat");
	}

	// 保存预知
	public void SavePrefab(string outputFilePath,Quaternion quaternion, string PrefabName)
	{
		GameObject g = new GameObject();
		g.name = PrefabName;
		g.transform.localRotation = quaternion;
		// mat 设定
		MeshRenderer mainMr = g.AddComponent<MeshRenderer>();
		mainMr.receiveShadows = false;
		mainMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mainMr.sharedMaterial = AssetDatabase.LoadAssetAtPath(outputFilePath + PrefabName + ".mat", typeof(Material)) as Material;
		// mesh 设定
		MeshFilter mainMf = g.AddComponent<MeshFilter>();
		mainMf.sharedMesh = AssetDatabase.LoadAssetAtPath(outputFilePath +  PrefabName + "_M.asset", typeof(Mesh)) as Mesh;
		//
		GpuAniMeshAction t = g.AddComponent<GpuAniMeshAction>();
		t.Fps = IGGUtil.framerate;
		//
		t.ListAniClip = new IGGAniClip[m_AniMesh.AniMeshClip.Count];
		for (int i = 0; i < m_AniMesh.AniMeshClip.Count; i++) 
		{
			t.ListAniClip [i].StartFrame = m_AniMesh.AniMeshClip [i].StartFrame;
			t.ListAniClip [i].EndFrame = m_AniMesh.AniMeshClip [i].EndFrame;
			t.ListAniClip [i].clipName = m_AniMesh.AniMeshClip [i].clipName;
		}
		// 保存预知
		string dataPath = outputFilePath +  PrefabName + ".prefab";
		PrefabUtility.CreatePrefab(dataPath, g, ReplacePrefabOptions.ConnectToPrefab);
	}
}