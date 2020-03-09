using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
/// <summary>
/// 资源加载模块 
/// </summary>
/// <author>zhulin</author>
/// Server--> Local (down) ---> Asset ====> gameobject / component

public delegate void ResLoadHook(GameObject go);

public class ResourceManger {

    private static MonoBehaviour g_asyncGo = null;
    public static MonoBehaviour AsyncGo {
        get { return g_asyncGo; }
    }
    private static AssetLoad g_AssetLoad = null;
    public static AssetLoad gAssetLoad {
        get { return g_AssetLoad; }
    }

    public static void SetLoadObj(MonoBehaviour fac, AssetLoad load) {
        g_AssetLoad = load;
        g_asyncGo = fac;
    }

    // 初始化资源模块
    public static void InitCache() {
        ABLoad.Init();
        g_AssetLoad.Init();
    }

    //统一的资源释放接口，有通过该模块加载的对象都应从该接口进行释放。
    public static void FreeGo(GameObject target, string ResType, bool immediate = false) {
        if (immediate) {
            GameObject.DestroyImmediate(target);
        } else {
            GameObject.Destroy(target);
        }
    }


    //   清理资源
    public static void Clear() {
        if (g_AssetLoad != null) {
            g_AssetLoad.Clear();
        }
        ABLoad.Clear();
    }


    // 加载预置，并不实例化
    public static void LoadAssetGo(string prefabName, string Type, bool IsCahce, AssetLoadHook pfun) {
        g_AssetLoad.LoadPrefab(prefabName, Type, IsCahce, true, pfun);
    }

    // 加载对象。
    private static void LoadGo(string ResName, string ResType, Transform parent, bool IsCacheAsset, bool async, ResLoadHook pfun) {
        GameObject go = null;
        g_AssetLoad.LoadPrefab(ResName, ResType, IsCacheAsset, async,
                (g) =>
                {
                    GameObject prefab = g as GameObject;
                    if (null != prefab) {
                        go = GameObject.Instantiate(prefab);
                        go.name = prefab.name;
                        // 设置父亲结点
                        if (null != go && null != parent) {
                            go.transform.SetParent(parent, false);
                        }
                    }
                    if (pfun != null) {
                        pfun(go);
                    }
                });
    }

    /// <summary>
    /// 加载场景item 
    /// <param ItemName>场景item名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadSceneItem(string ItemName, Transform parent, bool async, ResLoadHook pfun) {
        LoadGo(ItemName, ResourcesType.SceneItem, parent, true, async, pfun);
    }

    /// <summary>
    /// 加载窗口 
    /// <param WndName>窗口名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadWnd(string WndName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(WndName, ResourcesType.UIWnd, parent, IsCache, async, pfun);
    }

    /// <summary>
    /// 加载窗口item 
    /// <param WndItemName>窗口item名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadWndItem(string WndItemName, Transform parent, bool async, ResLoadHook pfun) {
        LoadGo(WndItemName, ResourcesType.UIWndItem, parent, true, async, pfun);
    }

    /// <summary>
    /// 加载特效
    /// <param EffectName>特效名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadEffect(string EffectName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(EffectName, ResourcesType.Effect, parent, IsCache, async, pfun);
    }

    /// <summary>
    /// 加载地图
    /// <param MapName>地图名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadMap(string MapName, Transform parent, bool async, ResLoadHook pfun) {
        LoadGo(MapName, ResourcesType.Map, parent, true, async, pfun);
    }

    /// <summary>
    /// 加载技能
    /// <param SkillName>技能名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadSkill(string SkillName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(SkillName, ResourcesType.Skill, parent, IsCache, async, pfun);
    }

    /// <summary>
    /// 加载小兵
    /// <param ActorName>小兵名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadActor(string ActorName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(ActorName, ResourcesType.ActorSoldier, parent, IsCache, async, pfun);
    }

    /// <summary>
    /// 加载英雄
    /// <param HeroName>英雄名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadHero(string HeroName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(HeroName, ResourcesType.ActorHero, parent, IsCache, async, pfun);
    }
    /// <summary>
    /// 加载英雄
    /// <param HeroName>英雄名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadShowHero(string HeroName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(HeroName, ResourcesType.ActorShowHero, parent, IsCache, async, pfun);
    }
    /// <summary>
    /// 加载建筑
    /// <param buildingName>建筑名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadBuilding(string buildingName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(buildingName, ResourcesType.Building, parent, IsCache, async, pfun);
    }



    /// <summary>
    /// 加载建筑模型
    /// <param buildingName>建筑名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadBuildingModel(string buildingName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(buildingName, ResourcesType.BuildingModel, parent, IsCache, async, pfun);
    }


    /// <summary>
    /// 加载树模型
    /// <param buildingName>树的名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadCityTree(string TreeName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(TreeName, ResourcesType.CityTree, parent, IsCache, async, pfun);
    }

    /// <summary>
    /// 加载建筑工地模型
    /// <param buildingName>建筑名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadBuildingSite(string buildingName, Transform parent, bool IsCache, bool async, ResLoadHook pfun) {
        LoadGo(buildingName, ResourcesType.BuildingSite, parent, IsCache, async, pfun);
    }


    /// <summary>
    /// 加载精灵
    /// <param AltasName>图集名称</param>
    /// <param SpriteName>精灵名称</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadSprite(string AtlasName, string SpriteName, bool async, AssetLoadHook pfun) {
        g_AssetLoad.LoadSprite(AtlasName, SpriteName, async, pfun);
    }


    /// <summary>
    /// 加载英雄动画
    /// <param HeroName>英雄名称</param>
    /// <param ClipName>动画名称</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadHeroAnim(string HeroName, string ClipName, bool async, AssetLoadHook pfun) {
        g_AssetLoad.LoadHeroAnim(HeroName, ClipName, async, pfun);
    }
    /// <summary>
    /// 加载音效
    /// <param audioName>audio</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadAudio(string audioName, bool async, AssetLoadHook pfun) {
        g_AssetLoad.LoadAudio(audioName, async, pfun);
    }

    public static void LoadSoldierMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
        g_AssetLoad.LoadActorMaterial(MaterialName, async, pfun);
    }

    public static void LoadSoldierAnim(string meshName, string animName, AssetLoadHook pfun) {
        meshName = meshName.Replace("_opponent", "");
        animName = animName.Replace("_opponent", "");
        g_AssetLoad.LoadSoldierAnim(meshName, animName, true, pfun);
    }

    

    // 读取lua
    public static byte[] loadLua() {
        return g_AssetLoad.LoadLua("fixlua", true);
    }



	public static void loadBuildingTexture(string TextureName, bool IsCahce, AssetLoadHook pfun) {
		 g_AssetLoad.loadBuildingTexture(TextureName, IsCahce, pfun);
	}

    public static void LoadPngTexture(string texName, bool isCache, AssetLoadHook fun)
    {
        g_AssetLoad.LoadPngTexture(texName, isCache, fun);
    }

    public static void LoadComMaterial(string MaterialName, bool async, AssetLoadHook pfun) {
        g_AssetLoad.LoadComMaterial(MaterialName, async, pfun);
    }

    /// <summary>
    /// 加载shader
    /// </summary>
    public static Shader LoadShader(string shaderName) {
        Shader s = null;
        g_AssetLoad.LoadShader(shaderName, false,
            (g) =>
            {
                s = g as Shader;
            });
        return s;
    }

    public static void LoadUI(string name)
    {
        g_AssetLoad.LoadUI(name);
    }

    public static Object LoadConfig(string name)
    {
        return g_AssetLoad.LoadConfig(name);
    }

    /// <summary>
    /// 加载场景
    /// <param ItemName>场景item名称</param>
    /// <param parent>父节点</param>
    /// <param IsCache>是否缓存</param>
    /// <param async>是否异步</param>
    /// <param pfun>回调函数</param>
    /// </summary>
    public static void LoadScene(string sceneName, SceneLoadHook pfun)
    {
        g_AssetLoad.LoadScene(sceneName, pfun);
    }

    public static void LoadDependeAB(string dir, string ABName) {
        ABLoad.LoadABbyPath(dir, ABName, true);
    }
}

