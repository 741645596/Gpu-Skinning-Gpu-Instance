using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ResVersion  {
	// 获取资源版本文件名
	public static string GetResVersionFileName(){
		return "ResVersion.txt";
	}

	// 解析资源版本。
	public static ResVersionInfo Analyse(string Path)
	{
		ResVersionInfo Info = new ResVersionInfo ();
		StreamReader sr ;
		try{
			sr = File.OpenText(Path);
		}
		catch(Exception e)
		{
			return null;
		}

		string line;
		while ((line = sr.ReadLine()) != null)
		{
			if(line.StartsWith("ABResVersion"))
			{
				Info.m_ABResVersion = int.Parse(line.Replace("ABResVersion = ",""));
			}
		}

		//关闭流
		sr.Close();
		//销毁流
		sr.Dispose();
		return Info;
	}


	// 获取需要更新的ab资源文件
	public static string GetABResUpdateFile(ResVersionInfo oldABVer, ResVersionInfo newABVer){

		if (newABVer == null)
			return string.Empty;

		int inewABVer = newABVer.m_ABResVersion;
		if (oldABVer == null) {
			if (inewABVer == 0) {
				return "ab.zip";
			} else {
				//return "ab" + newABVer.m_ABResVersion + ".zip";
				return "ab.zip";
			}
		} else {
			int ioldABVer = oldABVer.m_ABResVersion;
			if (inewABVer > ioldABVer) {
				return "ab" + inewABVer + "-" + ioldABVer + ".zip";
			}
			else return string.Empty;
			//return "ab.zip";
		}
	}
}

public class ResVersionInfo{
	public  int m_ABResVersion;

}
