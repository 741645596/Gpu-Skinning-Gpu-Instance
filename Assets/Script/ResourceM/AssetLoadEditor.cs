using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class AssetLoadEditor : AssetLoad{

	// 加载shader
	public override void LoadShader(string shaderName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.Shader, shaderName, "", typeof(Shader), true, pfun);
	}

	// 加载材质
	public  override void LoadActorMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.ActorSoldierMaterial, MaterialName, "", typeof(Material), true, pfun);
	}


	// 声音
	public override void LoadAudio(string audioName, bool async, AssetLoadHook pfun ) {
		LoadObj(ResourcesType.Audio, audioName, "", typeof(AudioClip), true, pfun);
	}

	// 加载预制
	public  override void LoadPrefab(string prefabName, string Type, bool IsCahce, bool async, AssetLoadHook pfun) {
		LoadObj(Type, prefabName, "", typeof(GameObject), IsCahce, pfun);
	}

	// 加载精灵
	public override void LoadSprite(string AtlasName ,string SpriteName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.UIAtlas, AtlasName, SpriteName, typeof(Sprite), true, pfun) ;
	}

	// 加载英雄动画
	public override void LoadHeroAnim(string HeroName ,string ClipName, bool async, AssetLoadHook pfun) {
		LoadObj(ResourcesType.HeroAnim, HeroName, ClipName, typeof(AnimationClip), true, pfun) ;
	}

	public override void loadBuildingTexture(string TextureName, bool IsCahce, AssetLoadHook pfun){
		LoadObj(ResourcesType.BuildingTexture, TextureName,"", typeof(Texture), IsCahce, pfun) ;
	}

    public override void LoadPngTexture(string texName, bool isCache, AssetLoadHook fun)
    {
        LoadObj(ResourcesType.PngTexture, texName, "", typeof(Texture), isCache, fun);
    }

	// 获取key 的接口
	private string GetKey(string ResType, string ObjName, string FileExt){
		string str = ResourcesPath.GetRelativePath(ResType, ResourcesPathMode.Editor);
		return str + ObjName + FileExt;
	}

    public override void LoadSoldierAnim(string meshName, string animName, bool isCache, AssetLoadHook pfun) {

        if(null == pfun) {
            return;
        }

		string FileExt = ResourcesPath.GetFileExt(ResourcesType.ActorSoldierMesh);
        string key = GetKey(ResourcesType.ActorSoldierMesh, meshName + "/" + animName, FileExt);

        Object obj = FindAssetObj(key);
        if (null != obj) {
            pfun.Invoke(obj);
        }else {
			string PathName = "Assets/" + ResourcesPath.GetRelativePath(ResourcesType.ActorSoldierMesh, ResourcesPathMode.Editor) + meshName + "/" + animName + FileExt;

            Texture2D ta = AssetDatabase.LoadAssetAtPath<Texture2D>(PathName) as Texture2D;

            if (null != ta) {
                if (isCache) {
                    AddCache(key, ta);
                }
                pfun.Invoke(ta);
            } else {
				Debug.Log("LoadAssetAtPath  Empty : " + key);
            }
        }
    }

    // 加载地图
    public override byte[] LoadMap(string MapName ,bool IsCache){
		string FileExt = ResourcesPath.GetFileExt (ResourcesType.MapData);
		string key = GetKey(ResourcesType.MapData, MapName, FileExt);
		byte[] AB = FindBytes(key);
		if (AB != null) { 
			return AB;
		} else {
			string PathName = "Assets/" + ResourcesPath.GetRelativePath (ResourcesType.MapData, ResourcesPathMode.Editor ) + MapName + FileExt;
			TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(PathName) as TextAsset;
			if (ta != null && IsCache == true) {
				AddBytesCache(key, ta.bytes);
				return ta.bytes;
			}
		}
		return null;
	}


	// 加载lua文件
	public override byte[] LoadLua(string luaName, bool IsCache){
		string FileExt = ResourcesPath.GetFileExt (ResourcesType.luaData);
		string key = GetKey(ResourcesType.luaData, luaName, FileExt);
		byte[] AB = FindBytes(key);
		if (AB != null) { 
			return AB;
		} else {
			string PathName = "Assets/" + ResourcesPath.GetRelativePath (ResourcesType.luaData, ResourcesPathMode.Editor ) + luaName + FileExt;
			TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(PathName) as TextAsset;
			if (ta != null && IsCache == true) {
				AddBytesCache(key, ta.bytes);
				return ta.bytes;
			}
		}
		return null;
	}





	// 加载asset接口
	private void LoadObj(string resType, 
		string objName,
		string subName, 
		System.Type type,  
		bool isCahce, 
		AssetLoadHook pfun)
	{
	    string fileExt;
		if (subName == "") {
			fileExt = ResourcesPath.GetFileExt (resType);
		} else {
			fileExt = "/" + subName + ResourcesPath.GetFileExt(resType);
		}
		string key = GetKey(resType, objName, fileExt);
        Object obj = FindAssetObj(key);
		if (obj != null) {
			if (pfun != null) {
				pfun(obj);
			}
		} else {
			string pathName = "Assets/" + ResourcesPath.GetRelativePath (resType, ResourcesPathMode.Editor ) + objName + fileExt;
			obj = AssetDatabase.LoadAssetAtPath(pathName,type);
			if ( obj != null && isCahce == true) {
				AddCache(key, obj);
			}
            else if(null == obj)
            {
				Debug.Log("LoadAssetAtPahth  Empty : " + pathName + "   Type :  " + type);
            }
			if (pfun != null) {
				pfun(obj);
			}
		}
	}

    public override void LoadComMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
        LoadObj(ResourcesType.ComMaterial, MaterialName, "", typeof(Material), async, pfun);
    }

    // 加载场景
    public override void LoadScene(string sceneName, SceneLoadHook pfun)
    {
        ResourceManger.AsyncGo.StartCoroutine(LoadSceneAsync(sceneName, pfun));
    }

    IEnumerator LoadSceneAsync(string sceneName, SceneLoadHook pfun)
    {
        AsyncOperation requestScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (requestScene.progress < 1)
        {
            if (pfun != null)
            {
                pfun(requestScene.progress);
            }

            yield return s_WaitForEndOfFrame;
        }

        if (pfun != null)
        {
            pfun(1f);
        }
    }
		

    // 加载配置文件
    public override Object LoadConfig(string name)
    {
        string path = ResourcesPath.GetRelativePath(ResourcesType.Config, ResourcesPathMode.Editor);
        string ext = ResourcesPath.GetFileExt(ResourcesType.Config);
        string fullpath = string.Format("Assets/{0}{1}.{2}", path, name, ext);

        return AssetDatabase.LoadAssetAtPath(fullpath, typeof(Object));
    }
}
#endif