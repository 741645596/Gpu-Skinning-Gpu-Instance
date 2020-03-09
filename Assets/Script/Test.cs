using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IGG;

public class Test : MonoBehaviour {

	private GUIStyle m_style;
	private GUIStyle m_style1;
	public GameObject CpuAniprefab;
	public GameObject CpuSkinprefab;
	public GameObject GpuAni3Rprefab;
	public GameObject GpuAniRGBAprefab;
	public GameObject GpuAniRGBMprefab;
	public GameObject GpuSkinprefab;
    public GameObject frameAni;

    private List<GameObject> m_Listgo = new List<GameObject>();
	public float m_w = 10;
	public float m_h = 10;



	private string m_text = "";
	// Use this for initialization
	void Start () {
		m_style = new GUIStyle ();
		m_style.normal.textColor = Color.red;
		m_style.normal.background = Texture2D.blackTexture;
		m_style.fontSize = 30;

		m_style1 = new GUIStyle ();
		m_style1.normal.textColor = Color.green;
		m_style1.normal.background = Texture2D.blackTexture;
		m_style1.fontSize = 30;
	}


	void OnGUI()
	{
		GUI.Label (new Rect (Screen.width /2 - 100, 0, 200, 40), m_text, m_style1);
		//
		GUI.Label (new Rect (Screen.width /2 - 100, 100, 200, 40), "Width", m_style1);
		m_w = GUI.HorizontalSlider (new Rect (Screen.width /2, 100, 300, 40), m_w, 1.0f, 40.0f);
		GUI.Label (new Rect (Screen.width /2 - 100, 150, 200, 40), "Height", m_style1);
		m_h = GUI.HorizontalSlider (new Rect (Screen.width /2, 150, 300, 40), m_h, 1.0f, 40.0f);


        GUI.Label(new Rect(Screen.width / 2 - 100, 0, 200, 40), "数量：" + m_Listgo.Count, m_style);
        // cpu animesh
        if (GUI.Button (new Rect (10, 10, 150, 60), "CpuAniMesh", m_style)) 
		{
			LoadPrefab (CpuAniprefab, "CpuAniMesh");
		}
		// cpu skin
		if (GUI.Button (new Rect (210, 10, 150, 60), "CpuSkin", m_style1)) 
		{
			LoadPrefab (CpuSkinprefab, "CpuSkin");
		}
		//
		if (GUI.Button (new Rect (10, 90, 150, 60), "Gpu3R", m_style)) 
		{
			LoadPrefab (GpuAni3Rprefab, "Gpu3R");
		}
		//
		if (GUI.Button (new Rect (210, 90, 150, 60), "GpuRGBA", m_style1)) 
		{
			LoadPrefab (GpuAniRGBAprefab, "GpuRGBA");
		}

		//
		if (GUI.Button (new Rect (410, 90, 150, 60), "GpuRGBM", m_style)) 
		{
			LoadPrefab (GpuAniRGBMprefab, "GpuRGBM");
		}
		//
		if (GUI.Button (new Rect (10, 170, 150, 60), "GpuSkin", m_style)) 
		{
			LoadPrefab (GpuSkinprefab, "GpuSkin");
		}
        //
        if (GUI.Button(new Rect(210, 170, 150, 60), "FrameAni", m_style))
        {
            LoadPrefab(frameAni, "FrameAni");
        }


        if (m_Listgo != null && m_Listgo.Count > 0) 
		{
            IAnimation ani = m_Listgo[0].GetComponent<IAnimation>();
            if (ani != null)
            {
                int i = 0;
                foreach (IGGAniClip clip in ani.ListAniClip)
                {
                    if (GUI.Button(new Rect(10, 250 + 80 * i, 250, 60), "播放动画：" + clip.clipName, m_style1))
                    {
                        PlayAni(clip.clipName);
                    }
                    i++;
                }
            }
		}
	}
	 
	private void  PlayAni(string AniName)
	{
		if (m_Listgo != null && m_Listgo.Count > 0) 
		{
            foreach (GameObject g in m_Listgo)
            {
                IAnimation ani = g.GetComponent<IAnimation>();
                if (ani != null)
                {
                    ani.Play(AniName);
                }
            }
        }
	}

	private void LoadPrefab(GameObject prefab, string typeName)
	{
		if (m_Listgo != null && m_Listgo.Count > 0) 
		{
			foreach(GameObject g in m_Listgo)
			{
				GameObject.Destroy (g);
			}
		}
		m_Listgo.Clear();
        //

		if (prefab != null) 
		{
			float dx = 6.0f;
			float dy = 6.0f;
			int w = (int)m_w;
			int h = (int)m_h;
			for (int i = 0; i < w; i++) 
			{
				for (int j = 0; j < h; j++)
				{
					GameObject go = Instantiate(prefab);
					m_text = typeName;
					go.name = m_text + "_" + go.name;
					go.transform.parent = GameObject.Find("ActorNode").transform;
					Vector3 v = new Vector3 ((i - w / 2) * dx, 0,  (j - h / 2) * dy) + go.transform.localPosition;
					go.transform.localPosition = v;
					IAnimation g = go.GetComponent<IAnimation>();
                    if (g != null)
                    {
                        g.Play(g.ListAniClip[0].clipName);
                        go.AddComponent<Rotate>();
                        m_Listgo.Add(go);
                    }
				}
			}

		}
	}

}
