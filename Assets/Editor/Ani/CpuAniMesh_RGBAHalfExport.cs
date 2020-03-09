using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CpuAniMesh_RGBAHalfExport : IAniMeshExport {

	private MeshAnimation m_AniMesh = null;
	// 设置mesh ani 数据
	public void SetAniMeshData(MeshAnimation aniMesh)
	{
		m_AniMesh = aniMesh;
	}


	public void Export(string outputFilePath,Quaternion quaternion, string AniMeshName)
	{
		string filePath = outputFilePath + "/CpuAniMeshRGBAHalf/";
		SaveMeshTexture(filePath, AniMeshName);
		SaveMesh(filePath, AniMeshName);
		SaveMaterial(filePath, AniMeshName);
		SavePrefab (filePath, quaternion, AniMeshName);
		EditorUtility.DisplayDialog("Tip", "Cpu Mesh Animation Export Complete" + m_AniMesh.m_ActorName, "OK");
		AssetDatabase.Refresh();
	}


	// 保存顶点动画纹理
	public void SaveMeshTexture(string outputFilePath, string AniMeshName)
	{
		//
		int totalFrame = m_AniMesh.GetTotalFrame();
		int totalvectors = m_AniMesh.GetTotalVertices();
		int npower = totalvectors;
		Texture2D combinedTex = new Texture2D(npower, totalFrame, TextureFormat.RGBAHalf, false);
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
				combinedTex.SetPixel(k,  frame, new Color(vertex.x, vertex.y, vertex.z));
				//Color c = IGGUtil.EncodeLogLuv(vertex);
				//combinedTex.SetPixel(k,  frame, c);
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
		mesh.vertices = m_AniMesh.m_Mesh.m_vertices;
		mesh.triangles = m_AniMesh.m_Mesh.m_Triangles;
		mesh.uv = m_AniMesh.m_Mesh.m_UV;
		mesh.RecalculateNormals();
		string dataPath = outputFilePath +  MeshName + "_M.asset";
		AssetDatabase.CreateAsset(mesh, dataPath);
	}


	// 保存mat文件
	public void SaveMaterial(string outputFilePath, string MaterialName)
	{
		Shader s = Shader.Find("CustomUnlitAlpha");
		Material mat = new Material(s);
		mat.name = MaterialName;
		mat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath("Assets/Models/Texture/" + MaterialName + ".tga", typeof(Texture2D)) as Texture2D);
		mat.SetColor("_GroupColor", Color.green);
		AssetDatabase.CreateAsset(mat, outputFilePath + MaterialName + ".mat");
	}


	// 保存预知
	public void SavePrefab(string outputFilePath,Quaternion quaternion, string PrefabName)
	{
		GameObject g = new GameObject();
		g.name = PrefabName;
		g.transform.localRotation = quaternion;
		// mesh 设定
		MeshFilter mainMf = g.AddComponent<MeshFilter>();
		mainMf.sharedMesh = AssetDatabase.LoadAssetAtPath(outputFilePath +  PrefabName + "_M.asset", typeof(Mesh)) as Mesh;
		// mat 设定
		MeshRenderer mainMr = g.AddComponent<MeshRenderer>();
		mainMr.receiveShadows = false;
		mainMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mainMr.sharedMaterial = AssetDatabase.LoadAssetAtPath(outputFilePath + PrefabName + ".mat", typeof(Material)) as Material;

		// cpu脚本
		AniMeshRGBAAction t = g.AddComponent<AniMeshRGBAAction>();
		t.meshTex = AssetDatabase.LoadAssetAtPath(outputFilePath +  PrefabName + "_AT.asset", typeof(Texture2D)) as Texture2D;
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
