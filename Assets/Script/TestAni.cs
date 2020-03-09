using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAni : MonoBehaviour
{
    private GUIStyle m_style;
    private GUIStyle m_style1;
    private float m_w = 100;
    private float m_h = 100;
    private int m_count = 0;

    private GameObject m_Ani = null;
    public GameObject Ani3D;
    public GameObject Ani2D;
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
        GUI.Label(new Rect(Screen.width / 2 - 100, 0, 200, 40), "数量：" + m_count, m_style);

        if (GUI.Button(new Rect(10, 10, 150, 60), "加载3D动画", m_style1))
        {
            if(m_Ani != null)
            {
                GameObject.DestroyImmediate(m_Ani);
                m_Ani = null;
            }
            m_Ani = Instantiate(Ani3D);
            TestInstance ins = m_Ani.GetComponent<TestInstance>();
            ins.m_w = m_w;
            ins.m_h = m_h;
            m_count = (int)m_w * (int)m_h;
        }

        if (GUI.Button(new Rect(10, 100, 150, 60), "加载2D动画", m_style1))
        {
            if (m_Ani != null)
            {
                GameObject.DestroyImmediate(m_Ani);
                m_Ani = null;
            }
            m_Ani = Instantiate(Ani2D);
            Ani2D ins = m_Ani.GetComponent<Ani2D>();
            ins.m_w = m_w;
            ins.m_h = m_h;
            m_count = (int)m_w * (int)m_h;
        }

        if (GUI.Button(new Rect(10, 190, 150, 60), "加载combine 2D动画", m_style1))
        {
            if (m_Ani != null)
            {
                GameObject.DestroyImmediate(m_Ani);
                m_Ani = null;
            }
            m_Ani = Instantiate(CombineAni2D);
            CombineMesh2DAni ins = m_Ani.GetComponent<CombineMesh2DAni>();
            ins.m_w = m_w /6 + 1;
            ins.m_h = m_h /6 + 1;
            m_count = (int)m_w * (int)m_h;
        }


        GUI.Label(new Rect(Screen.width / 2 - 100, 100, 200, 40), "Width", m_style);
        m_w = GUI.HorizontalSlider(new Rect(Screen.width / 2, 100, 400, 40), m_w, 1.0f, 100.0f);
        GUI.Label(new Rect(Screen.width / 2 - 100, 150, 200, 40), "Height", m_style);
        m_h = GUI.HorizontalSlider(new Rect(Screen.width / 2, 150, 400, 40), m_h, 1.0f, 100.0f);
    }
}
