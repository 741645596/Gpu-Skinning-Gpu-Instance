using UnityEngine;
using System.Collections;
using System.Text;

// <summary>
// 资源路径管理
// </summary>
// <author>zhulin</author>

// 集中路径模式
public enum ResourcesPathMode
{
    Editor,
    AssetBundle,
}

/// <summary>
/// use for ResFactory
/// create gameobject with all the type name and attach spawnpool component
/// </summary>
public struct ResourcesType
{

    public const string UIWnd = "UIWnd";
    public const string UIWndItem = "UIWndItem";
    public const string UIAtlas = "UIAtlas";
    public const string UIPublicWndAtlas = "UIPublicWndAtlas";
    public const string UIFairy = "UIFairy";
    public const string SceneItem = "SceneItem";

    // hero
    public const string ActorHero = "ActorHero";
    public const string ActorShowHero = "ActorShowHero";
    public const string HeroAnim = "HeroAnim";
    // soldier
    public const string ActorSoldier = "ActorSoldier";
    public const string ActorSoldierMaterial = "ActorSoldierMaterial";
    public const string ActorSoldierMesh = "ActorSoldierMesh";

    public const string Building = "Building";
    public const string BuildingModel = "BuildingModel";
    public const string BuildingSite = "BuildingSite";
    public const string CityTree = "CityTree";
    public const string BuildingTexture = "BuildingTexture";
    public const string PngTexture = "PngTexture";

    public const string Effect = "Effect";
    public const string Skill = "Skill";
    public const string Audio = "Audio";
    public const string Shader = "Shader";
    public const string Map = "Map";
    public const string Scene = "Scene";
    public const string MapData = "MapData";
    public const string luaData = "luaData";
    public const string ComMaterial = "CommonMaterial";

    public const string Depend = "Depend";

    public const string Config = "Config";
}

public class ResourcesPath  {
    private static readonly string[] ScenePath = { "Scene/", "scene/", ".unity" };
    private static readonly string[] LuaDataPath = { "Scripts/lua/", "lua/", ".lua" };

    private static readonly string[] UiWndPath = { "Data/wnd/panel/", "wnd/panel/", ".prefab"};
	private static readonly string[] UiItemsPath = { "Data/wnd/items/", "wnd/items/", ".prefab"};
    private static readonly string[] UIPublicAtlasPath = { "Data/Atlas/WndPublic/", "atlas/wndpublic/", ".png" };
    private static readonly string[] UIAtlasPath = { "Data/Atlas/Items/", "atlas/item/", ".png" };

    private static readonly string[] UIFairyPath = { "Data/ui/", "ui/", ".bytes" };
    private static readonly string[] AudioPath = { "Data/Audio/", "Audio/", ".ogg" };
    private static readonly string[] MapDataPath = { "Data/MapData/", "mapdata/", ".bytes" };
   
    // 配置文件
    private static readonly string[] ConfigPath = { "Data/config/", "config/", ".asset" };
    private static readonly string[] ShaderPath = { "Shader/Core/", "shader/", ".shader" };



    private static readonly string[] EffectPath = {"Prefabs/Effect/", "effect/", ".prefab"};
	private static readonly string[] BuildingPath = {"Prefabs/Building/", "building/", ".prefab"};
	private static readonly string[] BuildingModel = {"Prefabs/BuildingModel/", "buildingmodel/", ".prefab"};
	private static readonly string[] BuildingSite = {"Prefabs/BuildingSite/", "buildingsite/", ".prefab"};
	private static readonly string[] BuildingTexture = {"Models/Environment/Models/Environment/COL3Building/Textures/", "BuildingTexture/", ".tga"};
	private static readonly string[] CityTree = {"Prefabs/TreeBlock/", "tree/", ".prefab"};
    private static readonly string[] PngTexture = { "Textures/CommonPng/", "CommonTextures/Png/", ".png" };

    private static readonly string[] SkillPath = {"Prefabs/Skill/", "skill/", ".prefab"};
	private static readonly string[] MapPath = {"Prefabs/Map/", "map/", ".prefab"};
	private static readonly string[] SceneItemPath = {"Prefabs/sceneItem/", "SceneItem/", ".prefab"};

    // 小兵
    private static readonly string[] ActorSoldierMaterialPath = { "CpuAniMesh/Material/", "soldier/material/", ".mat"};
	private static readonly string[] ActorSoldierMeshPath = { "CpuAniMesh/MeshAnimation/", "soldier/meshanimation/", ".asset" };
    private static readonly string[] ActorSoldierPath = { "Prefabs/Soldier/", "soldier/prefab/" , ".prefab"};
	// 英雄
	private static readonly string[] ActorHeroPath = {"Prefabs/Actor/Hero/", "hero/prefab/", ".prefab"};
    private static readonly string[] ActorShowHeroPath = { "Prefabs/Actor/HeroShow/", "showhero/prefab/", ".prefab" };
    private static readonly string[] HeroAnimPath = {"Animations/Units/Hero/", "hero/anim/", ".anim"};

	// 通用材质
	private static readonly string[] CommonMaterialPath = { "Materials/Common/", "common/material/", ".mat" };


	//LightMapPath
	private static readonly string[] DependPath = { "depend", "depend/", ".prefab" };

    // 获取资源运行时路径
    public static string GetAssetResourceRunPath(string Type, ResourcesPathMode Mode)
    {
        StringBuilder Builder = new StringBuilder(string.Empty);
		if (Mode == ResourcesPathMode.AssetBundle) {
			Builder.Append (GetABRunDir ());
		} else {
			Builder.Append ("Assets/");
		}
		string str = GetRelativePath(Type, Mode);
		Builder.Append (str);
        return Builder.ToString();
    }




	private static string GetABRunDir()
    {
        return ConstantData.ABSavePath;
    }


	private static string[] GetResTypePath(string Type){

        if (Type == ResourcesType.UIWnd)
        {
            return UiWndPath;
        }
        else if (Type == ResourcesType.UIWndItem)
        {
            return UiItemsPath;
        }
        else if (Type == ResourcesType.UIAtlas)
        {
            return UIAtlasPath;
        }
        else if (Type == ResourcesType.ActorHero)
        {
            return ActorHeroPath;
        }
        else if (Type == ResourcesType.HeroAnim)
        {
            return HeroAnimPath;
        }
        else if (Type == ResourcesType.ActorShowHero)
        {
            return ActorShowHeroPath;
        }
        else if (Type == ResourcesType.ActorSoldier)
        {
            return ActorSoldierPath;
        }
        else if (Type == ResourcesType.ActorSoldierMaterial)
        {
            return ActorSoldierMaterialPath;
        }
        else if (Type == ResourcesType.ActorSoldierMesh)
        {
            return ActorSoldierMeshPath;
        }
        else if (Type == ResourcesType.Building)
        {
            return BuildingPath;
        }
        else if (Type == ResourcesType.BuildingModel)
        {
            return BuildingModel;
        }
        else if (Type == ResourcesType.BuildingSite)
        {
            return BuildingSite;
        }
        else if (Type == ResourcesType.CityTree)
        {
            return CityTree;
        }
        else if (Type == ResourcesType.BuildingTexture)
        {
            return BuildingTexture;
        }
        else if (Type == ResourcesType.PngTexture)
        {
            return PngTexture;
        }
        else if (Type == ResourcesType.Effect)
        {
            return EffectPath;
        }
        else if (Type == ResourcesType.Skill)
        {
            return SkillPath;
        }
        else if (Type == ResourcesType.SceneItem)
        {
            return SceneItemPath;
        }
        else if (Type == ResourcesType.Audio)
        {
            return AudioPath;
        }
        else if (Type == ResourcesType.Scene)
        {
            return ScenePath;
        }
        else if (Type == ResourcesType.UIPublicWndAtlas)
        {
            return UIPublicAtlasPath;
        }
        else if (Type == ResourcesType.Shader)
        {
            return ShaderPath;
        }
        else if (Type == ResourcesType.Map)
        {
            return MapPath;
        }
        else if (Type == ResourcesType.luaData)
        {
            return LuaDataPath;
        }
        else if (Type == ResourcesType.MapData)
        {
            return MapDataPath;
        }
        else if (Type == ResourcesType.ComMaterial)
        {
            return CommonMaterialPath;
        }
        else if (Type == ResourcesType.Depend)
        {
            return DependPath;
        }
        else if(Type == ResourcesType.UIFairy)
        {
            return UIFairyPath;
        }
        else if(Type == ResourcesType.Config)
        {
            return ConfigPath;
        }
		return null;
	}




	public static string GetRelativePath(string Type, ResourcesPathMode Mode){
		int index = (int)Mode;
		string[] sArray = GetResTypePath(Type);
		if (sArray == null || sArray.Length == 0)
			return string.Empty;
		return sArray[index];
	}


	public static string GetFileExt(string Type){

		string[] sArray = GetResTypePath(Type);
		if (sArray == null || sArray.Length < 3)
			return string.Empty;
		return sArray[2];
	}
}




