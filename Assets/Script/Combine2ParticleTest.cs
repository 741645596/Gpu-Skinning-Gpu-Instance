using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combine2ParticleTest : MonoBehaviour
{
    private GUIStyle m_style;
    private GUIStyle m_style1;

    private GameObject m_Ani = null;
    public GameObject Particle2D;
    public GameObject Particle2D2;
    public GameObject CombineAni2D;
    void Start()
    {
        m_style = new GUIStyle();
        m_style.normal.textColor = Color.red;
        m_style.normal.background = Texture2D.blackTexture;
        m_style.fontSize = 30;

        m_style1 = new GUIStyle();
        m_style1.normal.textColor = Color.green;
        m_style1.normal.background = Texture2D.blackTexture;
        m_style1.fontSize = 30;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 100, 0, 200, 40), "数量：10000", m_style);

        if (GUI.Button(new Rect(10, 10, 200, 60), "Particle方案", m_style1))
        {
            if (m_Ani != null)
            {
                GameObject.DestroyImmediate(m_Ani);
                m_Ani = null;
            }
            m_Ani = Instantiate(Particle2D);
        }

        if (GUI.Button(new Rect(10, 110, 200, 60), "Particle2方案", m_style1))
        {
            if (m_Ani != null)
            {
                GameObject.DestroyImmediate(m_Ani);
                m_Ani = null;
            }
            m_Ani = Instantiate(Particle2D2);
        }




        if (GUI.Button(new Rect(10, 200, 200, 60), "combineMesh方案", m_style1))
        {
            if (m_Ani != null)
            {
                GameObject.DestroyImmediate(m_Ani);
                m_Ani = null;
            }
            m_Ani = Instantiate(CombineAni2D);
            CombineMesh2DAni ins = m_Ani.GetComponent<CombineMesh2DAni>();
            ins.m_w = 17;
            ins.m_h = 17;
        }
    }
}