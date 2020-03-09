using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 加载asset 对象
/// </summary>
/// <author>zhulin</author>

public delegate void AssetLoadHook(UnityEngine.Object g);
public delegate void SceneLoadHook(float progress);
public class AssetLoad  {

	protected  Dictionary<string, UnityEngine.Object> g_CacheAsset = null;
	private    Dictionary<string,byte[]> g_Cachebytes = null;
    protected WaitForEndOfFrame s_WaitForEndOfFrame = new WaitForEndOfFrame();

    protected void AddCache(string key, UnityEngine.Object obj)
	{
		if (obj == null) {
			return;
		}

		if (g_CacheAsset.ContainsKey (key) == false) {
			g_CacheAsset.Add(key, obj);
		}
	}

	// 查找ab 包
	protected UnityEngine.Object FindAssetObj(string key){

		if (g_CacheAsset.ContainsKey (key) == true) {
			return g_CacheAsset [key];
		} else
			return null;
	}


	protected void AddBytesCache(string key, byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0) {
			return;
		}

		if (g_Cachebytes.ContainsKey (key) == false) {
			g_Cachebytes.Add(key, bytes);
		}
	}

	// 查找ab 包
	protected byte[] FindBytes(string key){

		if (g_Cachebytes.ContainsKey (key) == true) {
			return g_Cachebytes [key];
		} else
			return null;
	}



	// 初始化
	public  void Init()
	{
		g_CacheAsset = new Dictionary<string,  UnityEngine.Object>();
		g_Cachebytes = new Dictionary<string, byte[]>();
	}
		

	// 清理工作
	public void Clear()
	{
		g_CacheAsset.Clear();
		g_Cachebytes.Clear();
	}

	public virtual void FreeBytes(string ResType, string ABName){
		
	} 

	public virtual void FreeAsset(string ResType, string ABName){

	} 


	// 加载shader 的接口
	public virtual void LoadShader(string shaderName, bool async, AssetLoadHook pfun) {
	}


	public  virtual void LoadActorMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
	}


	// 声音
	public virtual void LoadAudio(string audioName, bool async, AssetLoadHook pfun ) {

	}


	// 加载精灵
	public virtual void LoadSprite(string AtlasName ,string SpriteName, bool async, AssetLoadHook pfun) {

	}


	// 加载英雄动画
	public virtual void LoadHeroAnim(string HeroName ,string ClipName, bool async, AssetLoadHook pfun) {

	}

	// 加载预制
	public  virtual void LoadPrefab(string prefabName, string Type, bool IsCahce, bool async, AssetLoadHook pfun) {
	}

    public virtual void LoadSoldierAnim(string meshName, string animName, bool IsCache, AssetLoadHook pfun) {
        
    }


	public virtual void loadBuildingTexture(string TextureName, bool IsCahce, AssetLoadHook pfun){
	}

    public virtual void LoadPngTexture(string texName, bool isCache, AssetLoadHook fun) { }

    // 加载地图
    public virtual byte[] LoadMap(string MapName ,bool IsCache){
		return null;
	}
		

    public virtual void LoadComMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
    }


	// 加载lua文件
	public virtual byte[] LoadLua(string LuaName, bool IsCache){
		return null;
	}

    // 加载场景
    public virtual void LoadScene(string sceneName, SceneLoadHook pfun)
    {

    }

    // 加载UI
    public virtual void LoadUI(string name)
    {

    }

    // 加载配置文件
    public virtual Object LoadConfig(string name)
    {
        return null;
    }
}
