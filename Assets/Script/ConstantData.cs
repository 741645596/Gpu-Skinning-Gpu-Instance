using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum VerContent
{
    FullVersion  = 0,  //完整版本
    IncreVersion = 1,  //增量版本
}


public class ConstantData
{
	public static VerContent g_VerContent = VerContent.FullVersion;

    // 将ab打成一个zip包
    public static bool AssetBundleUseZip = false;

    public static string ABSavePath{
			get { 
#if UNITY_STANDALONE_WIN
			return Application.streamingAssetsPath + "/ab/" + "/";
#else
			return Application.persistentDataPath + "/ab/"; 
#endif
		}
	}


	private static string StreamingAssetsPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			return "file:///" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
			return  Application.streamingAssetsPath + "/" ;
			//return "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
			return  "file:///" + Application.dataPath + "/Raw/";
#else
			return "file:///" + Application.streamingAssetsPath + "/";
#endif
		}
	 }




	public static string ABEXT{
		get{ return ".ab";}
	}
    /// <summary>
    /// 默认分辨率
    /// </summary>
    /// 
    public static float g_fDefaultResolutionWidth = 1920;
    public static float g_fDefaultResolutionHeight = 1080;

    public static string BattleDataPath {
        get
        {
			return Application.persistentDataPath + "/Battle/";
        }
    }



    public static string BattleReplayEXT
    {
        get { return ".Replay"; }
    }

	
	public static string ServerListServerPath {
		get
		{
			return "http://config.nos-eastchina1.126.net/";
		}
	}


	public static string ServerListSavePath {
		get
		{
		    return Application.persistentDataPath + "/";
		}
	}

	public static string ServerListFile {
		get
		{
		   return "serverlist.txt";
		}
	}



}
