using System.Collections.Generic;
using UnityEngine;

public class CombineMeshCreate
{
    private int m_width = 10;
    private int m_height = 10;
    private float m_Xstep = 100.0f;
    private float m_Zstep = 100.0f;
    /// <summary>
    /// 
    /// </summary>
    private List<Vector3> m_Listpt = new List<Vector3>();
    public List<Vector3> Listpt
    {
        get { return m_Listpt; }
    }
    private List<Vector2> m_Listuv = new List<Vector2>();
    private List<int> m_ListTriangle = new List<int>();
    public List<int> ListTriangle
    {
        get { return m_ListTriangle; }
    }
    /// <summary>
    /// 创建网格mesh
    /// </summary>
    /// <param name="start"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="xSize"></param>
    /// <param name="zSize"></param>
    /// <returns></returns>
    public Mesh CreateMesh(int w, int h, float xSize, float zSize, float ActorSize)
    {
        m_width = w;
        m_height = h;
        m_Xstep = xSize;
        m_Zstep = zSize;

        Vector3 c = new Vector3(m_width * xSize * 0.5f, 0, m_height * zSize * 0.5f);

        Mesh mesh = new Mesh();
        Clear();
        /// 顶点列表
        for (int z = 1; z <= m_height; z++)
        {
            for (int x = 1; x <= m_width; x++)
            {
                Vector3 CenterPos = CalcPos( x,  z, xSize, zSize) - c;
                CreateQuad(CenterPos, ActorSize);
            }
        }
        // 三角形索引列表。
        int linePointNum = m_width * 4;
        for (int z = 0; z < m_height; z++)
        {
            for (int x = 0; x < m_width; x++)
            {
                int start = z * linePointNum + x * 4;
                // 两个三角形。
                m_ListTriangle.Add(start);
                m_ListTriangle.Add(start + 1);
                m_ListTriangle.Add(start + 2);

                m_ListTriangle.Add(start + 2);
                m_ListTriangle.Add(start + 1);
                m_ListTriangle.Add(start + 3);
            }
        }
        mesh.vertices = m_Listpt.ToArray();
        mesh.uv = m_Listuv.ToArray();
        mesh.triangles = m_ListTriangle.ToArray();
        return mesh;
    }


    /// <summary>x
    /// 产生网格上的点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private void CreateQuad(Vector3 CentorPos, float size)
    {
        Vector3 P1 = new Vector3(CentorPos.x - size / 2, CentorPos.y, CentorPos.z);
        Vector3 P2 = new Vector3(CentorPos.x - size / 2, CentorPos.y + size, CentorPos.z);
        Vector3 P3 = new Vector3(CentorPos.x + size / 2, CentorPos.y, CentorPos.z);
        Vector3 P4 = new Vector3(CentorPos.x + size / 2, CentorPos.y + size, CentorPos.z);

        m_Listpt.Add(P1);
        m_Listpt.Add(P2);
        m_Listpt.Add(P3);
        m_Listpt.Add(P4);
        m_Listuv.Add(Vector2.zero);
        m_Listuv.Add(new Vector2(0.0f,1.0f));
        m_Listuv.Add(new Vector2(1.0f, 0.0f));
        m_Listuv.Add(Vector2.one);
    }



    /// <summary>x
    /// 产生UV
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private Vector3 CalcPos(int x, int z, float xSize, float zSize)
    {
        return new Vector3(xSize * x, 0, zSize * z);
    }

    public void Clear()
    {
        if (m_Listpt == null)
        {
            m_Listpt = new List<Vector3>();
        }
        m_Listpt.Clear();
        if (m_Listuv == null)
        {
            m_Listuv = new List<Vector2>();
        }
        m_Listuv.Clear();
        if (m_ListTriangle == null)
        {
            m_ListTriangle = new List<int>();
        }
        m_ListTriangle.Clear();
    }
}
