using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 共享材质管理
/// </summary>
/// <author>zhulin</author>

public class ShareMaterialM {

	//共享材质列表
	private static Dictionary<string, Material> g_ListMat = new Dictionary<string, Material>();
	// 材质定义关键字
	public static string GetKey(string Restype, string ResName, string Prop){
		return Restype + "+" + ResName + "+" + Prop;
	}

	/// <summary>
	/// 添加共享材质
	/// <param Restype>资源类型</param>
	/// <param Name>资源名称</param>
	/// <param Prop>资源属性</param>
	/// <param g>材质</param>
	/// </summary>
	public static void AddMaterial(string Restype, string Name, string Prop, Material g){
		string key = GetKey(Restype, Name, Prop);
		AddMaterial(key, g);
	}

    public static void RemoveMaterial(string key, bool needDestoryAsset) {
        if (g_ListMat.ContainsKey(key) == true) {
            Material mat = g_ListMat[key];
            GameObject.DestroyImmediate(mat, needDestoryAsset);
            g_ListMat.Remove(key);
        }
    }

	/// <summary>
	/// 添加共享材质
	/// <param key>材质关键字</param>
	/// <param g>材质</param>
	/// </summary>
	public static void AddMaterial(string key, Material g){
		if (g == null)
			return;

		if (g_ListMat.ContainsKey (key) == false) {
			g_ListMat.Add(key, g);
		}
	}

	// 获取共享材质
	public static Material GetMaterial(string Restype, string Name, string Prop, Material g){
		string key = GetKey(Restype, Name, Prop);
		return GetMaterial(key);
	}
	// 获取共享材质
	public static Material GetMaterial(string key){
		if (g_ListMat.ContainsKey (key) == true) {
			return g_ListMat[key];
		}
		return null;
	}

	// 清理销毁共享材质
	public static void Clear(){
		foreach (Material v in g_ListMat.Values) {
			GameObject.Destroy(v);
		}
		g_ListMat.Clear();
	}



}
