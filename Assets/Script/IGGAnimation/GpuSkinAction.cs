using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GpuSkinAction : IAnimation {

	private MeshRenderer render;
     // Use this for initialization
     void Start () {
		render = gameObject.GetComponent<MeshRenderer>();
    }


	public override void DoAnim(int Frame)
	{
		render.sharedMaterial.SetFloat("_frame", Frame);
	}
}