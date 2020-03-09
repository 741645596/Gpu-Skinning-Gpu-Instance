using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;


/// <summary>
/// 加载AB对象 
/// </summary>
/// <author>zhulin</author>
public delegate void ABLoadHook(AssetBundle ab);
public class ABLoad {

	// key 为相对目录，相对目录 + ab 文件名
	private static Dictionary<string,AssetBundle> g_CacheAB = null;

    // 初始化
    public static void Init()
    {
        g_CacheAB = new Dictionary<string, AssetBundle>();
    }

    // 清理工作
    public static void Clear()
    {
        foreach (AssetBundle ab in g_CacheAB.Values)
        {
            ab.Unload(true);
        }

        g_CacheAB.Clear();
    }

    // 加入缓存管理
    private static void AddABCache(string key, AssetBundle AB)
    {
        if (AB == null)
        {
            return;
        }

        if (g_CacheAB.ContainsKey(key) == false)
        {
            g_CacheAB.Add(key, AB);
        }
    }

    // 查找ab 包
    public static AssetBundle FindAB(string key)
    {

        if (g_CacheAB.ContainsKey(key) == true)
        {
            return g_CacheAB[key];
        }
        else
        {
            return null;
        }
    }

    /// 根据路径进行加载ab 包
    public static void LoadABbyPath(string RelativePath, string ABfileName, bool IsCache)
    {
        string ABPath = RelativePath + ABfileName;
        LoadAB(ABPath, IsCache, (ab) =>
        {
            if (ab != null)
            {
                ab.LoadAllAssets();
            }
        });
    }

    public static void LoadABbyPath(string path, bool IsCache)
    {
        LoadAB(path, IsCache, (ab) =>
        {
            if (ab != null)
            {
                ab.LoadAllAssets();
            }
        });
    }
		
    /// <summary>
    /// 异步的ab包加载
    /// 现在暂时是同步加载
    /// </summary>
    /// <param name="abPath"></param>
    /// <param name="isCache"></param>
    /// <param name="callback"></param>
	public static void LoadAB(string abPath, bool isCache, ABLoadHook callback)
	{
	    AssetBundle AB = LoadAbSync(abPath, isCache);
	    if (AB == null)
	    {
			Debug.Log("ab res load error: " + abPath);
	    }
		if (callback != null ) {
            callback(AB);
		}
    }

    // 获得AssetBundle文件所在的目录
    public static string GetAssetBundlePath(string path)
    {
        // 先从persistentDataPath读.如果不存在,则从streamingAssetsPath读
        string fullpath = string.Format("{0}/ab/{1}", Application.persistentDataPath, path);
        if (!File.Exists(fullpath))
        {
            fullpath = string.Format("{0}/ab/{1}", Application.streamingAssetsPath, path);
        }

        //Debug.LogFormat("GetAssetBundlePath {0} -> {1}", path, fullpath);

        return fullpath;
    }

    /// <summary>
    /// 同步的ab包加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isCache"></param>
    public static AssetBundle LoadAbSync(string path, bool isCache)
    {
        AssetBundle ab = FindAB(path);
        if (ab == null)
        {
            string abpath = GetAssetBundlePath(path);
            ab = AssetBundle.LoadFromFile(abpath);
            if (ab == null)
            {
                Debug.Log("ABLoad load ab fail :" + abpath);
            }
            else
            {
                if (isCache)
                {
                    AddABCache(path, ab);
                }
            }
        }
        return ab;
    }

    /// 异步加载AB
    public static IEnumerator LoadABasync(string ABPath, bool IsCache, ABLoadHook pfun)
    {
        AssetBundle ab = FindAB(ABPath);
        if (ab == null)
        {
            string abpath = GetAssetBundlePath(ABPath);

            AssetBundleCreateRequest ABquest = AssetBundle.LoadFromFileAsync(abpath);
            yield return ABquest;
            ab = ABquest.assetBundle;

            if (ab == null)
            {
                Debug.Log("ABLoad load ab fail :" + abpath);
            }
            else
            {
                if (IsCache)
                {
                    AddABCache(ABPath, ab);
                }
            }
        }

        if (pfun != null)
        {
            pfun(ab);
        }
    }

    public static byte[] Loadbytes(string path)
    {
        path = GetAssetBundlePath(path);

        return IGG.FileUtil.ReadByteFromFile(path);
    }

    // 获取相对目录，
    public static string GetRelativePath(string ResType, string ABName)
    {
        string path = ResourcesPath.GetRelativePath(ResType, ResourcesPathMode.AssetBundle);
        string fullname = string.Format("{0}{1}{2}", path, ABName, ConstantData.ABEXT);

        return fullname.ToLower();
    }


    // 释放ab 包
    public static void FreeAB(string ResType, string ABName)
    {
        string ABPath = GetRelativePath(ResType, ABName);
        AssetBundle AB = FindAB(ABPath);
        if (AB != null)
        {
            AB.Unload(true);
            g_CacheAB.Remove(ABPath);
        }
    }
}
