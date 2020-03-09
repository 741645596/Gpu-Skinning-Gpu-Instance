using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResFactory : MonoBehaviour {

	// Use this for initialization
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		// 资源加载
		InitResManger();
	}

	private void InitResManger(){
		AssetLoad l = null;
		#if UNITY_EDITOR
		l = new AssetLoadEditor();
		#else
		l = new AssetLoadRun();
		#endif
		ResourceManger.SetLoadObj(this, l);
		ResourceManger.InitCache();
	}
}
