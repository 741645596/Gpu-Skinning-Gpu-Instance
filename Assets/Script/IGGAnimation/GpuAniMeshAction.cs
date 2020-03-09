using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GpuAniMeshAction : IAnimation {

	private MeshRenderer render;
	// Use this for initialization
	void Start () {
		render = gameObject.GetComponent<MeshRenderer>();
	}


	public override void DoAnim(int Frame)
	{
		if (render.sharedMaterial.GetInt("_frame") != Frame) 
		{
			render.sharedMaterial.SetInt("_frame", Frame);
		}
	}
}