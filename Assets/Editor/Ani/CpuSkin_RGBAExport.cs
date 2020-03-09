using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CpuSkin_RGBAExport : IAniMeshExport {

	private MeshAnimation m_AniMesh = null;
	// 设置mesh ani 数据
	public void SetAniMeshData(MeshAnimation aniMesh)
	{
		m_AniMesh = aniMesh;
	}


	public void Export(string outputFilePath,Quaternion quaternion, string AniMeshName)
	{
		string filePath = outputFilePath + "/CpuSkin/";
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


		int bones = m_AniMesh.GetBones();
		int npower = bones * 3;
		Texture2D combinedTex = new Texture2D(npower, totalFrame, TextureFormat.RGBAFloat, false);
		combinedTex.filterMode = FilterMode.Point;
		combinedTex.wrapMode = TextureWrapMode.Clamp;
		combinedTex.anisoLevel = 0;
		//
		int vertexCount = 0;
		for (int frame = 0; frame < totalFrame; frame++) 
		{
			for(int k = 0; k < bones; k ++)
			{
				Matrix4x4 mat = m_AniMesh.m_Mesh.m_AniBone[frame][k];
				int index = 3 * k;
				combinedTex.SetPixel(index,  frame, new Color(mat.m00, mat.m01, mat.m02, mat.m03));
				combinedTex.SetPixel(index + 1,  frame, new Color(mat.m10, mat.m11, mat.m12, mat.m13));
				combinedTex.SetPixel(index + 2,  frame, new Color(mat.m20, mat.m21, mat.m22, mat.m23));
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
		// 切线，用于存储骨骼权重信息。每个顶点绑定2根骨骼。

		Vector4[] tangetsArray = new Vector4[totalvectors];
		Vector2[] bone3 = new Vector2[totalvectors];
		Vector2[] bone4 = new Vector2[totalvectors];
		for (int i = 0; i < totalvectors; i++) {
			tangetsArray[i].x = m_AniMesh.m_Mesh.m_BoneWeight[i].boneIndex0;
			tangetsArray[i].y = m_AniMesh.m_Mesh.m_BoneWeight[i].weight0;
			tangetsArray[i].z = m_AniMesh.m_Mesh.m_BoneWeight[i].boneIndex1;
			tangetsArray[i].w = m_AniMesh.m_Mesh.m_BoneWeight[i].weight1;

			bone3[i].x = m_AniMesh.m_Mesh.m_BoneWeight[i].boneIndex2;
			bone3[i].y = m_AniMesh.m_Mesh.m_BoneWeight[i].weight2;

			bone4[i].x = m_AniMesh.m_Mesh.m_BoneWeight[i].boneIndex3;
			bone4[i].y = m_AniMesh.m_Mesh.m_BoneWeight[i].weight3;
		}
		mesh.vertices = m_AniMesh.m_Mesh.m_vertices;

		mesh.triangles = m_AniMesh.m_Mesh.m_Triangles;

		mesh.uv  = m_AniMesh.m_Mesh.m_UV;

		mesh.uv2  = bone3;

		mesh.uv3  = bone4;

		mesh.tangents = tangetsArray;

		mesh.RecalculateNormals();

		string dataPath = outputFilePath +  MeshName + "_M.asset";
		AssetDatabase.CreateAsset(mesh, dataPath);
	}


	// 保存mat文件
	public void SaveMaterial(string outputFilePath, string MaterialName)
	{
		Shader s = Shader.Find("CpuCustomAlphaSkin");
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
		// mat 设定
		MeshRenderer mainMr = g.AddComponent<MeshRenderer>();
		mainMr.receiveShadows = false;
		mainMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mainMr.sharedMaterial = AssetDatabase.LoadAssetAtPath(outputFilePath + PrefabName + ".mat", typeof(Material)) as Material;
		// mesh 设定
		MeshFilter mainMf = g.AddComponent<MeshFilter>();
		mainMf.sharedMesh = AssetDatabase.LoadAssetAtPath(outputFilePath +  PrefabName + "_M.asset", typeof(Mesh)) as Mesh;
		//
		CpuSkinAction t = g.AddComponent<CpuSkinAction>();
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
		t.m_bones = m_AniMesh.GetBones();
		// 保存预知
		string dataPath = outputFilePath +  PrefabName + ".prefab";
		PrefabUtility.CreatePrefab(dataPath, g, ReplacePrefabOptions.ConnectToPrefab);
	}
}
