using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public interface IAniMeshExport
{
	// 设置mesh ani 数据
	void SetAniMeshData(MeshAnimation aniMesh);
	// 保存顶点动画纹理
	void SaveMeshTexture(string outputFilePath, string AniMeshName);
	// 保存mesh
	void SaveMesh(string outputFilePath, string MeshName);
	// 保存mat
	void SaveMaterial(string outputFilePath, string MaterialName);
	// 保存预知
	void SavePrefab(string outputFilePath,Quaternion quaternion, string PrefabName);
	// 导出操作
	void Export(string outputFilePath,Quaternion quaternion, string AniMeshName);
}
