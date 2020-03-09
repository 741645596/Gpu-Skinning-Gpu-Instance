using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniMeshRGBAAction : IAnimation {

	public Texture2D meshTex;
	private MeshFilter meshf;
	private Color[] _color;

	// Use this for initialization
	void Start () {
		_color = meshTex.GetPixels();
		meshf = gameObject.GetComponent<MeshFilter>();
	}



	public override void DoAnim(int Frame)
	{
		List<Vector3> l = new List<Vector3> ();
		for (int i = 0; i < meshTex.width; i++) {
			Color c = _color [meshTex.width * Frame + i];
			l.Add(ParamPos(c));
		}

		meshf.sharedMesh.vertices = l.ToArray();
		meshf.sharedMesh.RecalculateNormals();
	}
		

	private Vector3 ParamPos(Color c)
	{
		//return IGGUtil.DecodeLogLuv(c);
		Vector3 v = new Vector3(c.r, c.g, c.b);
	    return v;
	}
}
