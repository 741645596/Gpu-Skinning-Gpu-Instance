using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CpuSkinAction : IAnimation {

	public Texture2D meshTex;
	private Color[] _color;
	private MeshRenderer render;
	public int m_bones = 0;
	private Matrix4x4[] m_boneMat = null;


	// Use this for initialization
	void Start () {
		_color = meshTex.GetPixels();
		m_boneMat = new Matrix4x4[m_bones];
		render = gameObject.GetComponent<MeshRenderer>();
	}


	public override void DoAnim(int Frame)
	{
		for (int i = 0; i < m_bones; i++) {
			int index = 3 * i;
			Color c1 = _color [meshTex.width * Frame + index];
			Color c2 = _color [meshTex.width * Frame + index + 1];
			Color c3 = _color [meshTex.width * Frame + index + 2];

			m_boneMat [i].SetRow (0, new Vector4 (c1.r, c1.g, c1.b, c1.a));
			m_boneMat [i].SetRow (1, new Vector4 (c2.r, c2.g, c2.b, c2.a));
			m_boneMat [i].SetRow (2, new Vector4 (c3.r, c3.g, c3.b, c3.a));
			m_boneMat [i].SetRow (3, new Vector4 (0, 0, 0, 1));
		}

		render.sharedMaterial.SetMatrixArray("_Matrices", m_boneMat);
	}
}