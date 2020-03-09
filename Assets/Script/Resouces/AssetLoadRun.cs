using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetLoadRun : AssetLoad {

    // 加载shader 的接口
    public override void LoadShader(string shaderName, bool async, AssetLoadHook pfun) {
		string shaderkey = ResourcesPath.GetAssetResourceRunPath(ResourcesType.Shader,ResourcesPathMode.Editor) + shaderName + ".shader";
		shaderkey = shaderkey.ToLower();
		LoadObj(ResourcesType.Shader, "shader", shaderkey, typeof(Shader), true, true, false, async,pfun);
	}


	public  override void LoadActorMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.ActorSoldierMaterial, MaterialName.ToLower(), MaterialName.ToLower(), typeof(Material), true, true, false, async,pfun);
	}
		
	// 声音
	public override void LoadAudio(string audioName, bool async, AssetLoadHook pfun ) {
        string[] names = audioName.Split('/');
        string fileName = names[names.Length - 1].ToLower();
        LoadObj(ResourcesType.Audio, audioName, fileName, typeof(AudioClip), true, true, false, async,pfun) ;
	}


	// 加载精灵
	public override void LoadSprite(string AtlasName ,string SpriteName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.UIAtlas, AtlasName.ToLower(), SpriteName.ToLower(), typeof(Sprite), true, true, false, async,pfun);
	}

	// 加载英雄动画
	public override void LoadHeroAnim(string HeroName ,string ClipName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.HeroAnim, HeroName.ToLower(), ClipName.ToLower(), typeof(AnimationClip), true, true, false, async,pfun);
	}


	public override void loadBuildingTexture(string TextureName, bool IsCahce, AssetLoadHook pfun){
		LoadObj(ResourcesType.BuildingTexture, TextureName.ToLower(),TextureName.ToLower(), typeof(Texture), IsCahce,true, false, false, pfun) ;
	}

    public override void LoadPngTexture(string texName, bool isCache, AssetLoadHook fun)
    {
        LoadObj(ResourcesType.PngTexture, texName.ToLower(), texName.ToLower(), typeof(Texture), isCache, true, false, false, fun);
    }

    // 加载预制
    public  override void LoadPrefab(string prefabName, string Type , bool IsCahce, bool async, AssetLoadHook pfun) {
		string objName = prefabName.ToLower();
		if (IsCahce == true) {
			LoadObj(Type, objName, objName, typeof(GameObject), true, true, false, async, pfun);
		} else {
			LoadObj(Type, objName, objName, typeof(GameObject), false, false, true, async, pfun);
		}
	}

    // 加载mesh动画文件
    //Modify by Aoicocoon 2017/12/06
    //修改打包士兵数据+动画文件
    public override void LoadSoldierAnim(string meshName, string animName, bool IsCache, AssetLoadHook pfun) {

        if(null == pfun) {
            return;
        }

		string folder = ResourcesPath.GetRelativePath(ResourcesType.ActorSoldierMesh, ResourcesPathMode.AssetBundle);
        string key = GetKey(folder, meshName + "/" + animName.ToLower());
        string path = key + ConstantData.ABEXT;

        Object obj = FindAssetObj(key);

        if(null != obj) {
            pfun.Invoke(obj);
        } else {
            ABLoad.LoadAB(path, true,
                    (ab) =>
                    {
                        LoadQueue.AddLoadTask(path, animName, typeof(Texture2D), IsCache, IsCache, false, pfun);
                    });
        }
    }




	// 加载lua
	public override byte[] LoadLua(string luaName ,bool IsCache){
        byte[] bytes = null;

        string path = ABLoad.GetRelativePath(ResourcesType.luaData, "lua");
        AssetBundle ab = ABLoad.LoadAbSync(path, true);
        if (ab != null)
        {
            TextAsset asset = ab.LoadAsset<TextAsset>(luaName);
            if (asset != null)
            {
                bytes = asset.bytes;
            }
        }

        return bytes;
    }


		
	// 加载asset接口
	private void LoadObj(string ResType, 
		string ABName, 
		string ObjName, 
		Type type, 
		bool IsCacheAsset,
		bool IsCacheAB, 
		bool IsFreeUnUseABRes,
		bool async, 
		AssetLoadHook pfun)
	{
		string RelativePath = ABLoad.GetRelativePath(ResType, ABName);
		string key = GetKey(RelativePath, ObjName);
		UnityEngine.Object obj = FindAssetObj(key);
		if (obj != null) {
			if (pfun != null) {
				pfun (obj);
			}
		} else {
			if (async == false) {
				ABLoad.LoadAB(RelativePath, IsCacheAB, 
					(ab)=>{
                        Object g = LoadObjByAb (ab, ObjName, type, IsCacheAB, IsFreeUnUseABRes);
                        LoadAssetCallBack(g, key, IsCacheAsset, pfun);
                    });
			} else {
                // 异步加载队列
				LoadQueue.AddLoadTask(RelativePath, ObjName, type, IsCacheAsset, IsCacheAB, IsFreeUnUseABRes, pfun);
				
			}
		}
	}

    /// <summary>
    /// 以同步的方式加载资源 
    /// </summary>
    /// <param name="resType"></param>
    /// <param name="abName"></param>
    /// <param name="objName"></param>
    /// <param name="type"></param>
    /// <param name="isCache"></param>
    /// <param name="isCacheAb"></param>
    /// <param name="isFreeUnUseAbRes"></param>
    /// <returns></returns>
    private Object LoadObjSync(string resType, string abName, string objName, Type type,bool isCache,bool isCacheAb, bool isFreeUnUseAbRes)
    {
        string relativePath = ABLoad.GetRelativePath(resType, abName);
        string key = GetKey(relativePath, objName);
        Object obj = FindAssetObj(key);
        if (obj != null)
        {
            return obj;
        }

        AssetBundle assetBundle = ABLoad.LoadAbSync(relativePath, isCacheAb);
        if (assetBundle == null)
        {
            return null;
        }
       
        obj = LoadObjByAb(assetBundle, objName, type, isCacheAb, isFreeUnUseAbRes);
        if (isCache == true && obj != null)
        {
            AddCache(key, obj);
        }

        return obj;
    }

    // 获取key 的接口
    private string GetKey(string RelativePath, string ObjName){
		return RelativePath + ObjName;
	}

	// 获取key 的接口
	private string GetKey(string ResType, string ABName, string ObjName){
		string RelativePath = ABLoad.GetRelativePath(ResType, ABName);
		return GetKey(RelativePath, ObjName);
	}

	// 加载资源的回调
	public void LoadAssetCallBack(UnityEngine.Object obj, string key, bool IsCacheAsset, AssetLoadHook pfun){
		if (IsCacheAsset == true && obj != null) {
			AddCache (key, obj);
		}
		if (pfun != null) {
			pfun (obj);
		}
	}

    /// <summary>
    /// 从assetsBundle里加载资源
    /// </summary>
    /// <param name="ab"></param>
    /// <param name="objName"></param>
    /// <param name="type"></param>
    /// <param name="isCache"></param>
    /// <param name="isFreeUnUseAbRes"></param>
    /// <returns></returns>
	private Object LoadObjByAb(AssetBundle ab, string objName, Type type, bool isCache, bool isFreeUnUseAbRes){
	    if (ab == null)
	    {
	        return null;
	    }
        Object obj = ab.LoadAsset(objName, type);
        if (isFreeUnUseAbRes == true && isCache == false)
        {
            ab.Unload(false);
        }
        return obj;
	}

	public  IEnumerator LoadObjAsync(AssetBundle AB, string objName, System.Type type, bool IsCache, bool IsFreeUnUseABRes, AssetLoadHook pfun) {
		if (AB != null) {
			AssetBundleRequest quest = AB.LoadAssetAsync (objName, type);
			yield return quest;

			if (pfun != null) {
				pfun (quest.asset);
			}
			if (IsFreeUnUseABRes == true && IsCache == false) {
				AB.Unload (false);
			}
		} else {
			if (pfun != null) {
				pfun (null);
			}
		}
	}
    public override void LoadComMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
        string objName = MaterialName;
        LoadObj(ResourcesType.ComMaterial, MaterialName, objName, typeof(Material), true, true, false, async, pfun);
    }

    // 加载场景
    public override void LoadScene(string sceneName, SceneLoadHook pfun)
    {
        ResourceManger.AsyncGo.StartCoroutine(LoadSceneAsync(sceneName, pfun));
    }

    IEnumerator LoadSceneAsync(string sceneName, SceneLoadHook pfun)
    {
        string abPath = ABLoad.GetAssetBundlePath(ABLoad.GetRelativePath(ResourcesType.Scene, sceneName).ToLower());
        AssetBundleCreateRequest requestAB = AssetBundle.LoadFromFileAsync(abPath);

        while (requestAB.progress < 1)
        {
            if (pfun != null)
            {
                pfun(requestAB.progress * 0.5f);
            }

            yield return s_WaitForEndOfFrame;
        }

        AsyncOperation requestScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (requestScene.progress < 1)
        {
            if (pfun != null)
            {
                pfun(0.5f + requestScene.progress * 0.5f);
            }

            yield return s_WaitForEndOfFrame;
        }

        if (requestAB.assetBundle != null)
        {
            requestAB.assetBundle.Unload(false);
        }

        if (pfun != null)
        {
            pfun(1f);
        }
    }



    // 加载配置文件
    public override Object LoadConfig(string name)
    {
        Object data = null;

        string path = ABLoad.GetRelativePath(ResourcesType.Config, "config");
        AssetBundle ab = ABLoad.LoadAbSync(path, true);
        if(ab != null)
        {
            data = ab.LoadAsset<Object>(name);
        }

        return data;
    }
}
